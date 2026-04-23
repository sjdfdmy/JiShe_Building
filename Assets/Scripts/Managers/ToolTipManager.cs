using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance { get; private set; }

    [SerializeField] private GameObject tooltipPanel;   // 背景框
    [SerializeField] private TextMeshProUGUI tooltiptitle;          // 文字组件
    [SerializeField] private TextMeshProUGUI tooltipText;          // 文字组件
    [SerializeField] private RectTransform canvasRect;  // Canvas RectTransform
    [SerializeField] private Vector2 padding = new Vector2(20, 10);
    [SerializeField] public Vector2 offset = new Vector2(100, -15); // 相对鼠标偏移
    [SerializeField] public float showDelay = 0.5f; // 悬停延迟

    private RectTransform tooltipRect;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        tooltipRect = tooltipPanel.GetComponent<RectTransform>();
        Hide();
    }

    public void Show(string text)
    {
        tooltipText.text = text;
        tooltipPanel.SetActive(true);

        // 自适应大小
        Vector2 textSize = new Vector2(tooltipText.preferredWidth, tooltipText.preferredHeight);
        tooltipRect.sizeDelta = textSize + padding;
    }

    public void Show(string title,string text)
    {
        tooltiptitle.text = title;
        tooltipText.text = text;
        tooltipPanel.SetActive(true);

        // 自适应大小
        Vector2 textSize = new Vector2(tooltipText.preferredWidth, tooltipText.preferredHeight);
        tooltipRect.sizeDelta = textSize + padding;
    }

    public void Hide()
    {
        tooltipPanel.SetActive(false);
    }

    public void UpdatePosition(Vector2 screenPos)
    {
        // 屏幕坐标转局部坐标，防止超出屏幕
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPos, null, out Vector2 localPos);

        // 边界检测（简单版）
        float halfW = tooltipRect.sizeDelta.x * 0.5f;
        float halfH = tooltipRect.sizeDelta.y * 0.5f;
        float maxX = canvasRect.sizeDelta.x * 0.5f - halfW;
        float maxY = canvasRect.sizeDelta.y * 0.5f - halfH;

        localPos.x = Mathf.Clamp(localPos.x, -maxX, maxX);
        localPos.y = Mathf.Clamp(localPos.y, -maxY, maxY);

        tooltipRect.anchoredPosition = localPos;
    }
}