using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class GridItem : MonoBehaviour
{
    [Tooltip("Assign a MaterialData asset to auto-populate name, shape, and stats.")]
    public MaterialData materialData;

    public string itemName;
    [Tooltip("Include (0,0) for the pivot, and relative coordinates for children, e.g., (1,0)")]
    public List<Vector2Int> localCells = new List<Vector2Int> { new Vector2Int(0, 0) };

    [HideInInspector] public Vector2 spawnPosition;
    [HideInInspector] public Quaternion spawnRotation;
    [HideInInspector] public int currentRotationStep = 0;
    [HideInInspector] public Vector2Int currentGridPosition;
    [HideInInspector] public bool isInModule = false;

    private RectTransform rectTransform;
    private Image image;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        ApplyMaterialData();
    }

    /// <summary>
    /// Applies the assigned MaterialData to this GridItem, setting the item name
    /// and cell shape from the ScriptableObject so they don't need to be set manually.
    /// </summary>
    public void ApplyMaterialData()
    {
        if (materialData != null)
        {
            itemName = materialData.materialName;
            if (materialData.size != null && materialData.size.Count > 0)
                localCells = new List<Vector2Int>(materialData.size);
        }
    }

    void Start()
    {
        spawnPosition = rectTransform.anchoredPosition;
        spawnRotation = rectTransform.localRotation;
    }

    public RectTransform RectTransform
    {
        get
        {
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();
            return rectTransform;
        }
    }

    public Image Image
    {
        get
        {
            if (image == null)
                image = GetComponent<Image>();
            return image;
        }
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
                y = -temp - 1;
            }
            rotatedShape.Add(new Vector2Int(x, y));
        }
        return rotatedShape;
    }

    public void RotateItem()
    {
        currentRotationStep = (currentRotationStep + 1) % 4;
        rectTransform.localRotation = Quaternion.Euler(0, 0, -90f * currentRotationStep);
    }
}
