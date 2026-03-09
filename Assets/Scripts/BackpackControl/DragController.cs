using System.Collections;
using UnityEngine;

public class DragController : MonoBehaviour
{
    public InventoryGrid inventoryGrid;
    public LayerMask itemLayer; 
    public Material ghostValidMat; 
    public Material ghostInvalidMat; 

    private GridItem draggedItem;
    private GameObject ghostObject;
    private Plane dragPlane;
    private bool isDragging = false;
    
    private Vector3 mouseStartPos;
    private bool wasInBackpackBeforeDrag;

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
                if (ghostObject) ghostObject.transform.rotation = draggedItem.transform.rotation;
            }
        }
    }

    void StartDrag()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, itemLayer))
        {
            draggedItem = hit.collider.GetComponent<GridItem>();
            if (draggedItem != null)
            {
                isDragging = true;
                mouseStartPos = Input.mousePosition;
                wasInBackpackBeforeDrag = draggedItem.isInBackpack;
                
                inventoryGrid.RemoveItem(draggedItem); 

                dragPlane = new Plane(Vector3.forward, new Vector3(0, 0, draggedItem.transform.position.z));
                CreateGhost();
            }
        }
    }

    void UpdateDrag()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            draggedItem.transform.position = hitPoint;

            UpdateGhostPreview();
        }
    }

    void EndDrag()
    {
        isDragging = false;
        if (ghostObject) Destroy(ghostObject);

        Vector2Int gridPos = inventoryGrid.WorldToGrid(draggedItem.transform.position);
        
        bool isClick = Vector3.Distance(Input.mousePosition, mouseStartPos) < 5f;

        if (isClick && wasInBackpackBeforeDrag)
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
        ghostObject = Instantiate(draggedItem.gameObject);
        Destroy(ghostObject.GetComponent<GridItem>());
        Destroy(ghostObject.GetComponent<Collider>());
        
        ghostObject.SetActive(false); 
    }

    void UpdateGhostPreview()
    {
        Vector2Int gridPos = inventoryGrid.WorldToGrid(draggedItem.transform.position);

        if (inventoryGrid.IsWithinBounds(draggedItem, gridPos))
        {
            if (!ghostObject.activeSelf) ghostObject.SetActive(true);
            
            ghostObject.transform.position = inventoryGrid.GridToWorld(gridPos);

            if (inventoryGrid.IsPlacementValid(draggedItem, gridPos))
                ChangeGhostMaterial(ghostValidMat);
            else
                ChangeGhostMaterial(ghostInvalidMat);
        }
        else
        {
            if (ghostObject.activeSelf) ghostObject.SetActive(false);
        }
    }

    void ChangeGhostMaterial(Material mat)
    {
        Renderer[] renderers = ghostObject.GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            r.material = mat;
        }
    }

    IEnumerator SmoothReturn(GridItem item)
    {
        Vector3 startPos = item.transform.position;
        Quaternion startRot = item.transform.rotation;
        
        float time = 0;
        float duration = 0.25f;

        item.currentRotationStep = 0; 

        while (time < duration)
        {
            item.transform.position = Vector3.Lerp(startPos, item.spawnPosition, time / duration);
            item.transform.rotation = Quaternion.Lerp(startRot, item.spawnRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        item.transform.position = item.spawnPosition;
        item.transform.rotation = item.spawnRotation;
    }
}