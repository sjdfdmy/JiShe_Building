using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class InventoryGrid : MonoBehaviour
{
    public int width = 4;
    public int height = 4;
    public float cellSize = 80f;
    public RectTransform gridOrigin;

    [Header("Cover Skill")]
    public bool hasCoverSkill = false;
    public int maxSharedSlots = 1;

    private GridItem[,] gridData;
    private GridItem[,] overlapData;
    private List<GridItem> itemsInModule = new List<GridItem>();
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        gridData = new GridItem[width, height];
        overlapData = new GridItem[width, height];
    }

    public Vector2Int PositionToGrid(Vector2 anchoredPos)
    {
        Vector2 offset = anchoredPos - gridOrigin.anchoredPosition;
        int x = Mathf.RoundToInt(offset.x / cellSize);
        int y = Mathf.RoundToInt(offset.y / cellSize);
        return new Vector2Int(x, y);
    }

    public Vector2 GridToPosition(Vector2Int gridPos)
    {
        return gridOrigin.anchoredPosition + new Vector2(gridPos.x * cellSize, gridPos.y * cellSize);
    }
    
    public bool IsWithinBounds(GridItem item, Vector2Int pivotPos)
    {
        foreach (Vector2Int cell in item.GetCurrentShape())
        {
            int targetX = pivotPos.x + cell.x;
            int targetY = pivotPos.y + cell.y;

            if (targetX < 0 || targetX >= width || targetY < 0 || targetY >= height)
                return false;
        }
        return true;
    }

    public bool IsPlacementValid(GridItem item, Vector2Int pivotPos)
    {
        foreach (Vector2Int cell in item.GetCurrentShape())
        {
            int targetX = pivotPos.x + cell.x;
            int targetY = pivotPos.y + cell.y;

            if (targetX < 0 || targetX >= width || targetY < 0 || targetY >= height)
                return false;

            GridItem occupant = gridData[targetX, targetY];
            if (occupant != null && occupant != item)
            {
                if (!hasCoverSkill)
                    return false;
                
                GridItem overlapOccupant = overlapData[targetX, targetY];
                if (overlapOccupant != null && overlapOccupant != item)
                    return false;
            }
        }

        if (hasCoverSkill)
        {
            int newShared = CountNewSharedSlots(item, pivotPos);
            if (newShared > 0 && GetCurrentSharedSlotCount() + newShared > maxSharedSlots)
                return false;
        }

        return true;
    }
    
    public int CountNewSharedSlots(GridItem item, Vector2Int pivotPos)
    {
        int count = 0;
        foreach (Vector2Int cell in item.GetCurrentShape())
        {
            int targetX = pivotPos.x + cell.x;
            int targetY = pivotPos.y + cell.y;

            if (targetX < 0 || targetX >= width || targetY < 0 || targetY >= height)
                continue;

            GridItem occupant = gridData[targetX, targetY];
            if (occupant != null && occupant != item)
                count++;
        }
        return count;
    }

    private int GetCurrentSharedSlotCount()
    {
        int count = 0;
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (gridData[x, y] != null && overlapData[x, y] != null)
                    count++;
        return count;
    }

    public void PlaceItem(GridItem item, Vector2Int pivotPos)
    {
        RemoveItem(item); 
        
        foreach (Vector2Int cell in item.GetCurrentShape())
        {
            int x = pivotPos.x + cell.x;
            int y = pivotPos.y + cell.y;

            if (gridData[x, y] == null)
                gridData[x, y] = item;
            else
                overlapData[x, y] = item; // Shared slot (cover skill)
        }
        
        item.currentGridPosition = pivotPos;
        item.RectTransform.anchoredPosition = GridToPosition(pivotPos);
        item.isInModule = true; // Mark as in module
        
        if (!itemsInModule.Contains(item))
            itemsInModule.Add(item);
    }

    public void RemoveItem(GridItem item)
    {
        if (itemsInModule.Contains(item))
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (overlapData[x, y] == item)
                    {
                        overlapData[x, y] = null;
                    }
                    else if (gridData[x, y] == item)
                    {
                        // Promote the overlap item to primary if present
                        gridData[x, y] = overlapData[x, y];
                        overlapData[x, y] = null;
                    }
                }
            }
            itemsInModule.Remove(item);
            item.isInModule = false;
        }
    }

    public Dictionary<string, Vector2Int> ExportInventory()
    {
        Dictionary<string, Vector2Int> exportedData = new Dictionary<string, Vector2Int>();
        foreach (GridItem item in itemsInModule)
        {
            exportedData.Add(item.itemName, item.currentGridPosition);
        }
        return exportedData;
    }

    /// <summary>
    /// Draws the module grid in the Scene view using Gizmos.
    /// Useful during testing to visually align the grid origin with
    /// the module texture.
    /// </summary>
    void OnDrawGizmos()
    {
        if (gridOrigin == null) return;

        // Convert the grid origin anchored position to world space
        Vector3 originWorld = gridOrigin.position;
        // Determine world-space size of one cell
        Vector3 cellWorld = gridOrigin.TransformVector(new Vector3(cellSize, cellSize, 0f));
        float cellW = Mathf.Abs(cellWorld.x);
        float cellH = Mathf.Abs(cellWorld.y);

        Gizmos.color = Color.cyan;

        // Draw vertical lines
        for (int x = 0; x <= width; x++)
        {
            Vector3 bottom = originWorld + gridOrigin.right * (x * cellW);
            Vector3 top = bottom + gridOrigin.up * (height * cellH);
            Gizmos.DrawLine(bottom, top);
        }

        // Draw horizontal lines
        for (int y = 0; y <= height; y++)
        {
            Vector3 left = originWorld + gridOrigin.up * (y * cellH);
            Vector3 right = left + gridOrigin.right * (width * cellW);
            Gizmos.DrawLine(left, right);
        }

        // Highlight the origin cell
        Gizmos.color = Color.yellow;
        Vector3 originCenter = originWorld + gridOrigin.right * (cellW * 0.5f) + gridOrigin.up * (cellH * 0.5f);
        Gizmos.DrawWireCube(originCenter, new Vector3(cellW, cellH, 0f));
    }

    /// <summary>
    /// Adjusts the slot (cell) size of the module so it can fit the visual
    /// texture of the UI. Updates the grid panel size and repositions all
    /// currently placed items to match the new cell size.
    /// </summary>
    /// <param name="newCellSize">New cell size in pixels.</param>
    public void SetSlotSize(float newCellSize)
    {
        if (newCellSize <= 0f)
        {
            Debug.LogWarning("SetSlotSize: newCellSize must be greater than 0.");
            return;
        }

        cellSize = newCellSize;

        // Resize the grid panel to match the new cell size
        rectTransform.sizeDelta = new Vector2(width * cellSize, height * cellSize);

        // Reposition all placed items to align with the new cell size
        foreach (GridItem item in itemsInModule)
        {
            item.RectTransform.anchoredPosition = GridToPosition(item.currentGridPosition);
        }
    }
}