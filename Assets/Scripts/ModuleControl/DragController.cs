using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragController : MonoBehaviour
{
    public InventoryGrid inventoryGrid;
    public Canvas canvas;
    public Color ghostValidColor = new Color(0f, 1f, 0f, 0.5f);
    public Color ghostInvalidColor = new Color(1f, 0f, 0f, 0.5f);
    public Color ghostSharedColor = new Color(1f, 1f, 0f, 0.5f);

    private GridItem draggedItem;
    private GameObject ghostObject;
    private bool isDragging = false;

    private Vector2 mouseStartPos;
    private bool wasInModuleBeforeDrag;
    private Camera uiCamera;

    void Start()
    {
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
        uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            StartDrag();
        else if (Input.GetMouseButtonUp(0) && isDragging)
            EndDrag();

        if (isDragging)
        {
            UpdateDrag();
            if (Input.GetKeyDown(KeyCode.E))
            {
                draggedItem.RotateItem();
                if (ghostObject)
                    ghostObject.GetComponent<RectTransform>().localRotation = draggedItem.RectTransform.localRotation;
            }
        }
    }

    void StartDrag()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            GridItem item = result.gameObject.GetComponent<GridItem>();
            if (item != null)
            {
                draggedItem = item;
                isDragging = true;
                mouseStartPos = Input.mousePosition;
                wasInModuleBeforeDrag = draggedItem.isInModule;

                inventoryGrid.RemoveItem(draggedItem);
                draggedItem.RectTransform.SetAsLastSibling();
                CreateGhost();
                break;
            }
        }
    }

    void UpdateDrag()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            uiCamera,
            out Vector2 localPoint);

        draggedItem.RectTransform.anchoredPosition = localPoint;
        UpdateGhostPreview();
    }

    void EndDrag()
    {
        isDragging = false;
        if (ghostObject) Destroy(ghostObject);
        
        Vector2Int gridPos = inventoryGrid.PositionToGrid(draggedItem.RectTransform.anchoredPosition);

        bool isClick = Vector2.Distance(Input.mousePosition, mouseStartPos) < 5f;

        if (isClick && wasInModuleBeforeDrag)
        {
            StartCoroutine(SmoothReturn(draggedItem));
        }
        else if (inventoryGrid.IsWithinBounds(draggedItem, gridPos) && inventoryGrid.IsPlacementValid(draggedItem, gridPos))
        {
            inventoryGrid.PlaceItem(draggedItem, gridPos);
        }
        else
        {
            StartCoroutine(SmoothReturn(draggedItem));
        }

        draggedItem = null;
    }

    void CreateGhost()
    {
        ghostObject = Instantiate(draggedItem.gameObject, draggedItem.RectTransform.parent);
        Destroy(ghostObject.GetComponent<GridItem>());

        // Place the ghost below the dragged item so it doesn't appear above the block
        int draggedIndex = draggedItem.RectTransform.GetSiblingIndex();
        ghostObject.GetComponent<RectTransform>().SetSiblingIndex(draggedIndex);

        // Disable raycasting on all ghost images so they don't intercept input
        Image[] images = ghostObject.GetComponentsInChildren<Image>();
        foreach (var img in images)
            img.raycastTarget = false;

        ghostObject.SetActive(false);
    }

    void UpdateGhostPreview()
    {
        Vector2Int gridPos = inventoryGrid.PositionToGrid(draggedItem.RectTransform.anchoredPosition);

        if (inventoryGrid.IsWithinBounds(draggedItem, gridPos))
        {
            if (!ghostObject.activeSelf) ghostObject.SetActive(true);

            ghostObject.GetComponent<RectTransform>().anchoredPosition = inventoryGrid.GridToPosition(gridPos);

            if (inventoryGrid.IsPlacementValid(draggedItem, gridPos))
            {
                if (inventoryGrid.hasCoverSkill && inventoryGrid.CountNewSharedSlots(draggedItem, gridPos) > 0)
                    ChangeGhostColor(ghostSharedColor);
                else
                    ChangeGhostColor(ghostValidColor);
            }
            else
                ChangeGhostColor(ghostInvalidColor);
        }
        else
        {
            if (ghostObject.activeSelf) ghostObject.SetActive(false);
        }
    }

    void ChangeGhostColor(Color color)
    {
        Image[] images = ghostObject.GetComponentsInChildren<Image>();
        foreach (var img in images)
        {
            img.color = color;
        }
    }

    IEnumerator SmoothReturn(GridItem item)
    {
        Vector2 startPos = item.RectTransform.anchoredPosition;
        Quaternion startRot = item.RectTransform.localRotation;

        float time = 0;
        float duration = 0.25f;

        item.currentRotationStep = 0;
        Quaternion targetRot = item.spawnRotation;

        while (time < duration)
        {
            item.RectTransform.anchoredPosition = Vector2.Lerp(startPos, item.spawnPosition, time / duration);
            item.RectTransform.localRotation = Quaternion.Lerp(startRot, targetRot, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        item.RectTransform.anchoredPosition = item.spawnPosition;
        item.RectTransform.localRotation = targetRot;
    }
}
