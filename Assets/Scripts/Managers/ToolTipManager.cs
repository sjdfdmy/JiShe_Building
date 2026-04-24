using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance { get; private set; }

    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TextMeshProUGUI tooltiptitle;
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private Vector2 padding = new Vector2(20, 10);
    [SerializeField] public Vector2 offset = new Vector2(15, -15);
    [SerializeField] public float showDelay = 0.5f;

    private RectTransform tooltipRect;
    private Canvas parentCanvas;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        tooltipRect = tooltipPanel.GetComponent<RectTransform>();

        // 🔴 关键：动态获取 Canvas，不再依赖手动赋值
        parentCanvas = tooltipRect.GetComponentInParent<Canvas>();

        // 设置 pivot 为左上角，方便定位
        tooltipRect.pivot = new Vector2(0f, 1f);
        tooltipRect.anchorMin = Vector2.zero;
        tooltipRect.anchorMax = Vector2.zero;

        Hide();
    }

    public void Show(string text)
    {
        tooltiptitle.gameObject.SetActive(false);
        tooltipText.text = text;
        tooltipPanel.SetActive(true);
        RefreshSize();
    }

    public void Show(string title, string text)
    {
        tooltiptitle.gameObject.SetActive(true);
        tooltiptitle.text = title;
        tooltipText.text = text;
        tooltipPanel.SetActive(true);
        RefreshSize();
    }

    void RefreshSize()
    {
        Canvas.ForceUpdateCanvases();
        float titleHeight = tooltiptitle.gameObject.activeSelf ? tooltiptitle.preferredHeight : 0;
        Vector2 textSize = new Vector2(
            Mathf.Max(tooltiptitle.preferredWidth, tooltipText.preferredWidth),
            titleHeight + tooltipText.preferredHeight + (titleHeight > 0 ? 5 : 0)
        );
        tooltipRect.sizeDelta = textSize + padding;
    }

    public void Hide()
    {
        tooltipPanel.SetActive(false);
    }

    public void UpdatePosition(Vector2 screenPos)
    {
        // 计算目标屏幕位置
        Vector2 targetScreenPos = screenPos + offset;

        // 获取 tooltip 的屏幕尺寸
        float w = tooltipRect.sizeDelta.x;
        float h = tooltipRect.sizeDelta.y;

        // 🔴 边界检测：在屏幕空间直接做，避免坐标转换误差
        if (targetScreenPos.x + w > Screen.width)
            targetScreenPos.x = screenPos.x - w - offset.x; // 超出右边界，翻转到左侧

        if (targetScreenPos.y - h < 0)
            targetScreenPos.y = screenPos.y + h + Mathf.Abs(offset.y); // 超出下边界，翻转到上方

        if (targetScreenPos.x < 0) targetScreenPos.x = 0;
        if (targetScreenPos.y > Screen.height) targetScreenPos.y = Screen.height;

        // 🔴 获取正确的 Camera
        Camera cam = null;
        if (parentCanvas != null && parentCanvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            cam = parentCanvas.worldCamera;
        }

        // 🔴 转换到 tooltip 父物体的局部坐标（而不是某个随机的 canvasRect）
        RectTransform parent = tooltipRect.parent as RectTransform;
        if (parent != null && RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parent, targetScreenPos, cam, out Vector2 localPos))
        {
            tooltipRect.anchoredPosition = localPos;
        }
        else
        {
            // 兜底：如果转换失败，直接用屏幕坐标（适用于 Overlay 模式）
            tooltipRect.anchoredPosition = targetScreenPos;
        }
    }
}