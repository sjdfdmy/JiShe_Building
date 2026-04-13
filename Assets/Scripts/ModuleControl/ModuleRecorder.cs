using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds the recorded data for a single material placed in the module.
/// </summary>
[Serializable]
public class MaterialEntry
{
    public string itemName;
    public MaterialKind kind;
    public Vector2Int position;
    public int rotationStep;

    public MaterialEntry(string itemName, MaterialKind kind, Vector2Int position, int rotationStep)
    {
        this.itemName = itemName;
        this.kind = kind;
        this.position = position;
        this.rotationStep = rotationStep;
    }
}

/// <summary>
/// Snapshot of the entire module at the time the record button was pressed.
/// Contains every placed material and the aggregated elegance / stability totals.
///
/// Access from any script:
///   ModuleRecord record = ModuleRecorder.LastRecord;
/// </summary>
[Serializable]
public class ModuleRecord
{
    public List<MaterialEntry> materials = new List<MaterialEntry>();
    public float totalElegance;
    public float totalStability;
}

/// <summary>
/// Attach this component to a UI Button. When the button is clicked (call
/// <see cref="RecordAndClear"/>), it snapshots every placed material's name,
/// kind, grid position, rotation step, and the sums of elegance and stability,
/// then clears the module so it is ready for the next layout.
///
/// The most recent record is always available through the static property
/// <see cref="LastRecord"/>, making it trivially accessible from any script
/// without needing a direct reference:
///
///   ModuleRecord record = ModuleRecorder.LastRecord;
///   if (record != null)
///       Debug.Log("Total elegance: " + record.totalElegance);
/// </summary>
public class ModuleRecorder : MonoBehaviour
{
    [Tooltip("Reference to the InventoryGrid that holds the placed materials.")]
    public InventoryGrid inventoryGrid;

    [Tooltip("Optional popup UI that displays elegance / stability / sum bars after recording.")]
    public ModuleRecordUI recordUI;

    /// <summary>
    /// The most recent module record. Accessible from any script via
    /// ModuleRecorder.LastRecord. Null until the first recording is made.
    /// </summary>
    public static ModuleRecord LastRecord { get; private set; }

    /// <summary>
    /// Wire this method to a UI Button's OnClick event.
    /// Records every placed material and then clears the module.
    /// </summary>
    public void RecordAndClear()
    {
        if (inventoryGrid == null)
        {
            Debug.LogWarning("[ModuleRecorder] InventoryGrid reference is not assigned.");
            return;
        }

        List<GridItem> items = inventoryGrid.GetItemsInModule();

        // Do nothing when the module is empty
        if (items.Count == 0)
        {
            Debug.Log("[ModuleRecorder] Module is empty – nothing to record.");
            return;
        }

        ModuleRecord record = new ModuleRecord();

        foreach (GridItem item in items)
        {
            MaterialKind kind = default;
            float elegance = 0f;
            float stability = 0f;

            if (item.materialData != null)
            {
                kind = item.materialData.kind;
                elegance = item.materialData.elegance;
                stability = item.materialData.stability;
            }

            record.materials.Add(new MaterialEntry(
                item.itemName,
                kind,
                item.currentGridPosition,
                item.currentRotationStep
            ));

            record.totalElegance += elegance;
            record.totalStability += stability;
        }

        LastRecord = record;
        inventoryGrid.ClearAllItems();

        Debug.Log($"[ModuleRecorder] Recorded {record.materials.Count} material(s). " +
                  $"Elegance: {record.totalElegance}, Stability: {record.totalStability}");

        // Show the result popup UI if assigned
        if (recordUI != null)
            recordUI.Show(record.totalElegance, record.totalStability);
    }
}
