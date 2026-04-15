using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public enum MaterialKind
{
    Building,
    Dye,
    Ornament
}

[CreateAssetMenu(fileName = "Material", menuName = "CreateData/Material", order = 1)]
public class MaterialData : ScriptableObject
{
    public string materialName;

    [Tooltip("The kind of this material (wall, ceiling, etc.)")]
    public MaterialKind kind;
    [Range(1, 3)]
    public int level;
    public int value;
    [Range(0, 100)]
    public int simvalravity;
    [Range(0, 100)]
    public int highvalravity;

    [Tooltip("The shape of this material in grid cells. Include (0,0) for the pivot and relative coordinates for additional cells.")]
    public List<Vector2Int> size = new List<Vector2Int> { new Vector2Int(0, 0) };

    [Tooltip("Elegance value used for calculating the output of the module.")]
    public float elegance;

    [Tooltip("Stability value used for calculating the output of the module.")]
    public float stability;
}
