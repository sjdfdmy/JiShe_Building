using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("设置")]
    public BlockType blockType;
    public float snapDistance = 80f;      // 吸附距离阈值
    public float returnDuration = 0.3f;    // 回弹动画时间

    [Header("状态")]
    public bool isPlaced = false;          // 是否已放置

    // 私有变量
    private Vector3 startPosition;         // 初始位置（原位）
    private Transform startParent;
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    // 目标吸附点（由GameManager设置）
    [HideInInspector] public Target targetSnap;

    public enum BlockType
    {
        Horizontal,    // 横条
        LeftLeg,       // 左腿
        RightLeg       // 右腿
    }

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        // 找到Canvas
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            canvas = FindObjectOfType<Canvas>();
    }

    void Start()
    {
        // 记录初始位置
        startPosition = rectTransform.anchoredPosition;
        startParent = transform.parent;

        // 查找对应的目标点
        FindTargetSnap();
    }

    void FindTargetSnap()
    {
        Target[] allTargets = FindObjectsOfType<Target>();
        foreach (var target in allTargets)
        {
            if (target.acceptedType == blockType)
            {
                targetSnap = target;
                break;
            }
        }
    }

   
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isPlaced) return;  // 已放置的不能拖动

        // 拖动时稍微透明，并且穿透点击（防止阻挡射线）
        canvasGroup.alpha = 0.8f;
        canvasGroup.blocksRaycasts = false;

        // 移到最前
        transform.SetAsLastSibling();

       
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isPlaced) return;

        // 跟随鼠标移动
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out localPoint);

        rectTransform.anchoredPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isPlaced) return;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // 检查是否在吸附范围内
        if (targetSnap != null && CanSnap())
        {
            SnapToTarget();
        }
        else
        {
            ReturnToStart();
        }
    }

    bool CanSnap()
    {
        // 计算与目标点的距离（屏幕空间或本地空间）
        float distance = Vector2.Distance(rectTransform.anchoredPosition,
                                         targetSnap.GetComponent<RectTransform>().anchoredPosition);
        return distance <= snapDistance;
    }

    void SnapToTarget()
    {
        // 吸附到目标位置
        RectTransform targetRect = targetSnap.GetComponent<RectTransform>();

        // 使用DoTween或协程做平滑动画
        StartCoroutine(SmoothMove(targetRect.anchoredPosition, () => {
            // 动画完成后
            isPlaced = true;
            targetSnap.OnBlockPlaced(this);

            // 检查是否完成所有
            GameManager.Instance?.CheckComplete();
        }));
    }

    void ReturnToStart()
    {
        StartCoroutine(SmoothMove(startPosition, null));
       
    }

    System.Collections.IEnumerator SmoothMove(Vector2 targetPos, System.Action onComplete)
    {
        Vector2 startPos = rectTransform.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / returnDuration;
            // 缓动曲线
            t = Mathf.SmoothStep(0, 1, t);

            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        rectTransform.anchoredPosition = targetPos;
        onComplete?.Invoke();
    }

    // 重置（用于重新开始）
    public void ResetBlock()
    {
        isPlaced = false;
        rectTransform.anchoredPosition = startPosition;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }
}