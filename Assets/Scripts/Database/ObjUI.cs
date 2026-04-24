using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler//, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum Type
    {
        inbag,
        pickobj,
        pickbag,
        store
    }
    public MaterialData obj;//预制体属性
    public Type type;//物品状态（先不用管）
    public bool nums;//启用堆叠
    public int num;//堆叠数量
    private TextMeshProUGUI numtext;//堆叠数量的文本
    private GameObject stars;
    private Image back;

    [SerializeField] private Canvas parentCanvas;
    [SerializeField] private bool fadeOriginal = true;
    [SerializeField] private float fadeAlpha = 0.4f;

    private GameObject dragPrefab;
    private GameObject dragInstance;
    private RectTransform dragRect;
    private CanvasGroup canvasGroup;

    private float enterTime;
    private bool isHovering;
    private bool isShown;

    // 拖拽状态记录
    private Vector2 originalAnchoredPos;
    private Transform originalParent;
    private InventoryGrid currentGrid;
    private GridItem gridItem;

    public void OnPointerEnter(PointerEventData eventData)
    {
        enterTime = Time.time;
        isHovering = true;
        isShown = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        if(TooltipManager.Instance != null)
        TooltipManager.Instance.Hide();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (TooltipManager.Instance != null)
            if (!isShown)
        {
            if (Time.time - enterTime >= TooltipManager.Instance.showDelay)
            {
                Show(eventData);
            }
        }
        else
        {
            TooltipManager.Instance.UpdatePosition(eventData.position + TooltipManager.Instance.offset);
        }
    }

    private void Show(PointerEventData eventData)
    {
        isShown = true;
        if (TooltipManager.Instance != null)
            if (string.IsNullOrEmpty(obj.materialName))
            TooltipManager.Instance.Show(obj.description);
        else
            TooltipManager.Instance.Show(obj.materialName, obj.description);
        if (TooltipManager.Instance != null)
            TooltipManager.Instance.UpdatePosition(eventData.position + TooltipManager.Instance.offset);
    }

    void Start()
    {
        Init();
    }

    public void Init()
    {
        if(numtext == null)
        {
            numtext = transform.GetComponentInChildren<TextMeshProUGUI>();
        }
        if (dragPrefab == null)
        {
            dragPrefab = obj.ModuleUI;
        }
        if (parentCanvas == null)
            parentCanvas = GetComponentInParent<Canvas>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null && fadeOriginal)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (stars == null)
        {
            stars = transform.GetChild(4).gameObject;
            for (int i = 0; i <= 2; i++)
            {
                stars.transform.GetChild(i).gameObject.SetActive(i < obj.level);
            }
        }

        if (back == null)
        {
            back = transform.GetChild(0).GetComponent<Image>();
        }

        gridItem = GetComponent<GridItem>();
        if (gridItem == null)
        {
            gridItem = gameObject.AddComponent<GridItem>();
            gridItem.materialData = obj;
            gridItem.ApplyMaterialData();
        }

        Button btn = gameObject.AddComponent<Button>();
        switch (type)
        {
            case Type.inbag:
                nums = true;
                break;
            case Type.pickobj:
                break;
            case Type.pickbag:
                break;
            case Type.store:
                break;
        }
    }

    //public void OnBeginDrag(PointerEventData eventData)
    //{
    //    if (dragPrefab == null || parentCanvas == null) return;

    //    RectTransform rt = GetComponent<RectTransform>();
    //    originalAnchoredPos = rt.anchoredPosition;
    //    originalParent = transform.parent;

    //    if (canvasGroup == null)
    //        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    //    canvasGroup.blocksRaycasts = false;

    //    currentGrid = FindCurrentGrid();
    //    if (currentGrid != null && gridItem != null)
    //    {
    //        currentGrid.RemoveItem(gridItem);
    //        transform.SetParent(originalParent, false);
    //        rt.anchoredPosition = originalAnchoredPos;
    //    }

    //    // 创建拖拽副本（原逻辑不变）
    //    dragInstance = Instantiate(dragPrefab, parentCanvas.transform);
    //    dragInstance.transform.SetAsLastSibling();

    //    dragRect = dragInstance.GetComponent<RectTransform>();
    //    if (dragRect == null)
    //        dragRect = dragInstance.AddComponent<RectTransform>();

    //    dragRect.pivot = new Vector2(0.5f, 0.5f);
    //    UpdateDragPosition(eventData);

    //    if (fadeOriginal && canvasGroup != null)
    //        canvasGroup.alpha = fadeAlpha;
    //}

    //public void OnDrag(PointerEventData eventData)
    //{
    //    if (dragInstance != null)
    //        UpdateDragPosition(eventData);
    //}

    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    // 恢复透明度
    //    if (fadeOriginal && canvasGroup != null)
    //        canvasGroup.alpha = 1f;

    //    // 🔴 新增：恢复射线检测，让原物体重新响应鼠标
    //    if (canvasGroup != null)
    //        canvasGroup.blocksRaycasts = true;

    //    InventoryGrid targetGrid = GetGridUnderPointer(eventData);


    //    if (targetGrid != null && gridItem != null)
    //    {
    //        // 计算网格坐标
    //        Vector2Int gridPos = targetGrid.PositionToGrid(eventData.position);

    //        // 检测是否可放置
    //        if (targetGrid.IsPlacementValid(gridItem, gridPos))
    //        {
    //            // ✅ 放置成功
    //            PlaceToGrid(targetGrid, gridPos);
    //        }
    //        else
    //        {
    //            // ❌ 不能放置，返回原位置
    //            ReturnToOriginal();
    //        }
    //    }
    //    else
    //    {
    //        // ❌ 没有网格，返回原位置
    //        ReturnToOriginal();
    //    }

    //    // 🔴 关键修改：不销毁拖拽体，留在原地；只断开引用
    //    dragInstance = null;
    //}

    //// ========== 辅助方法 ==========

    //void UpdateDragPosition(PointerEventData eventData)
    //{
    //    Vector2 localPos;
    //    RectTransformUtility.ScreenPointToLocalPointInRectangle(
    //        parentCanvas.transform as RectTransform,
    //        eventData.position,
    //        parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera,
    //        out localPos);

    //    dragRect.anchoredPosition = localPos;
    //}

    //// 获取鼠标下方的 InventoryGrid
    //InventoryGrid GetGridUnderPointer(PointerEventData eventData)
    //{
    //    List<RaycastResult> results = new List<RaycastResult>();
    //    EventSystem.current.RaycastAll(eventData, results);

    //    foreach (RaycastResult result in results)
    //    {
    //        InventoryGrid grid = result.gameObject.GetComponent<InventoryGrid>();
    //        if (grid != null)
    //            return grid;
    //    }
    //    return null;
    //}

    //// 查找当前所在的网格（通过父物体）
    //InventoryGrid FindCurrentGrid()
    //{
    //    Transform check = transform.parent;
    //    while (check != null)
    //    {
    //        InventoryGrid grid = check.GetComponent<InventoryGrid>();
    //        if (grid != null) return grid;
    //        check = check.parent;
    //    }
    //    return null;
    //}

    //// 放置到目标网格
    //void PlaceToGrid(InventoryGrid grid, Vector2Int pos)
    //{
    //    currentGrid = grid;

    //    // 设置父物体为网格
    //    transform.SetParent(grid.transform);

    //    // 使用 GridItem 的 RectTransform 属性
    //    gridItem.RectTransform.anchoredPosition = grid.GridToPosition(pos);

    //    // 调用网格放置
    //    grid.PlaceItem(gridItem, pos);
    //}

    //// 返回原位置
    //void ReturnToOriginal()
    //{
    //    transform.SetParent(originalParent);
    //    GetComponent<RectTransform>().anchoredPosition = originalAnchoredPos;

    //    // 如果原来有网格，放回去
    //    if (currentGrid != null && gridItem != null)
    //    {
    //        currentGrid.PlaceItem(gridItem, gridItem.currentGridPosition);
    //    }
    //}

    void Update()
    {
        if (nums&& GameDataManager.Instance.bags.Find(x => x.objdata == obj)!=null)
        {
            num=GameDataManager.Instance.bags.Find(x=> x.objdata == obj).num;
        }
        if (numtext != null)
        {
            numtext.gameObject.SetActive(nums);
            if (nums)
                numtext.text = num.ToString();
        }
    }

    void Move()
    {
    }
}