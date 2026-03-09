using System.Collections.Generic;
using UnityEngine;

public class GridItem : MonoBehaviour
{
    public string itemName;
    [Tooltip("Include (0,0) for the pivot, and relative coordinates for children, e.g., (1,0)")]
    public List<Vector2Int> localCells = new List<Vector2Int> { new Vector2Int(0, 0) };
    
    [HideInInspector] public Vector3 spawnPosition;
    [HideInInspector] public Quaternion spawnRotation;
    [HideInInspector] public int currentRotationStep = 0;
    [HideInInspector] public Vector2Int currentGridPosition;
    [HideInInspector] public bool isInBackpack = false;

    void Start()
    {
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
    }

    public List<Vector2Int> GetCurrentShape()
    {
        List<Vector2Int> rotatedShape = new List<Vector2Int>();
        foreach (var cell in localCells)
        {
            int x = cell.x;
            int y = cell.y;
            
            for (int i = 0; i < currentRotationStep; i++)
            {
                int temp = x;
                x = y;
                y = -temp;
            }
            rotatedShape.Add(new Vector2Int(x, y));
        }
        return rotatedShape;
    }

    public void RotateItem()
    {
        currentRotationStep = (currentRotationStep + 1) % 4;
        transform.Rotate(0, 0, -90f);
    }
}