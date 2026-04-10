using System.Collections.Generic;
using UnityEngine;

public enum MaterialKind
{
    Wall,
    Ceiling,
    Floor,
    Pillar,
    Beam,
    Roof,
    Window,
    Door
}

[CreateAssetMenu(fileName = "Material", menuName = "CreateData/Material", order = 1)]
public class MaterialData : ScriptableObject
{
    public string materialName;

    [Tooltip("The kind of this material (wall, ceiling, etc.)")]
    public MaterialKind kind;

    [Tooltip("The shape of this material in grid cells. Include (0,0) for the pivot and relative coordinates for additional cells.")]
    public List<Vector2Int> size = new List<Vector2Int> { new Vector2Int(0, 0) };

    [Tooltip("Elegance value used for calculating the output of the module.")]
    public float elegance;

    [Tooltip("Stability value used for calculating the output of the module.")]
    public float stability;
}
