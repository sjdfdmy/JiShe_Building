using System.Collections.Generic;
using UnityEngine;

public class InventoryGrid : MonoBehaviour
{
    public int width = 4;
    public int height = 4;
    public float cellSize = 1f;
    public Transform gridOrigin; 

    private GridItem[,] gridData;
    private List<GridItem> itemsInBackpack = new List<GridItem>();

    void Awake()
    {
        gridData = new GridItem[width, height];
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        Vector3 localPos = worldPos - gridOrigin.position;
        int x = Mathf.RoundToInt(localPos.x / cellSize);
        int y = Mathf.RoundToInt(localPos.y / cellSize);
        return new Vector2Int(x, y);
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return gridOrigin.position + new Vector3(gridPos.x * cellSize, gridPos.y * cellSize, 0);
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
            
            if (gridData[targetX, targetY] != null && gridData[targetX, targetY] != item)
                return false;
        }
        return true;
    }

    public void PlaceItem(GridItem item, Vector2Int pivotPos)
    {
        RemoveItem(item); 
        
        foreach (Vector2Int cell in item.GetCurrentShape())
        {
            gridData[pivotPos.x + cell.x, pivotPos.y + cell.y] = item;
        }
        
        item.currentGridPosition = pivotPos;
        item.transform.position = GridToWorld(pivotPos);
        item.isInBackpack = true; // Mark as in backpack
        
        if (!itemsInBackpack.Contains(item))
            itemsInBackpack.Add(item);
    }

    public void RemoveItem(GridItem item)
    {
        if (itemsInBackpack.Contains(item))
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (gridData[x, y] == item) gridData[x, y] = null;
                }
            }
            itemsInBackpack.Remove(item);
            item.isInBackpack = false;
        }
    }

    public Dictionary<string, Vector2Int> ExportInventory()
    {
        Dictionary<string, Vector2Int> exportedData = new Dictionary<string, Vector2Int>();
        foreach (GridItem item in itemsInBackpack)
        {
            exportedData.Add(item.itemName, item.currentGridPosition);
        }
        return exportedData;
    }
}