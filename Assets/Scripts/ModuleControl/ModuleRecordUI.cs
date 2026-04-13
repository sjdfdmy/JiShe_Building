using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays a popup UI after recording a module, showing three bars
/// (elegance, stability, sum) scaled by configurable max values,
/// along with text fields displaying the recorded numeric values.
///
/// Assign this component to the root GameObject of the popup panel.
/// The panel should be inactive by default; <see cref="Show"/> will
/// activate it and update the visuals.
/// </summary>
public class ModuleRecordUI : MonoBehaviour
{
    [Header("Max Values")]
    [Tooltip("Maximum elegance value used to normalize the elegance bar.")]
    public float maxElegance = 100f;

    [Tooltip("Maximum stability value used to normalize the stability bar.")]
    public float maxStability = 100f;

    [Tooltip("Maximum sum value used to normalize the sum bar.")]
    public float maxSum = 200f;

    [Header("Bar Images (fill or stretch)")]
    [Tooltip("Image whose width represents the elegance value.")]
    public RectTransform eleganceBar;

    [Tooltip("Image whose width represents the stability value.")]
    public RectTransform stabilityBar;

    [Tooltip("Image whose width represents the sum value.")]
    public RectTransform sumBar;

    [Header("Value Texts")]
    [Tooltip("Text field that displays the elegance value.")]
    public TMP_Text eleganceText;

    [Tooltip("Text field that displays the stability value.")]
    public TMP_Text stabilityText;

    [Tooltip("Text field that displays the sum value.")]
    public TMP_Text sumText;

    [Header("Bar Settings")]
    [Tooltip("The full width (in pixels) a bar should have when the value equals its max.")]
    public float barFullWidth = 200f;

    /// <summary>
    /// Activates the popup and updates bars and text fields
    /// based on the given recorded values.
    /// </summary>
    /// <param name="elegance">Recorded elegance value.</param>
    /// <param name="stability">Recorded stability value.</param>
    public void Show(float elegance, float stability)
    {
        float sum = elegance + stability;

        // Update bar widths proportionally
        SetBarWidth(eleganceBar, elegance, maxElegance);
        SetBarWidth(stabilityBar, stability, maxStability);
        SetBarWidth(sumBar, sum, maxSum);

        // Update text fields
        if (eleganceText != null) eleganceText.text = elegance.ToString("F1");
        if (stabilityText != null) stabilityText.text = stability.ToString("F1");
        if (sumText != null) sumText.text = sum.ToString("F1");

        gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the popup panel.
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void SetBarWidth(RectTransform bar, float value, float maxValue)
    {
        if (bar == null) return;

        float ratio = maxValue > 0f ? Mathf.Clamp01(value / maxValue) : 0f;
        Vector2 size = bar.sizeDelta;
        size.x = ratio * barFullWidth;
        bar.sizeDelta = size;
    }
}
