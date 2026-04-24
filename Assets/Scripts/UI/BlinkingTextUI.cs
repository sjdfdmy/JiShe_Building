using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class BlinkingTextUI : MonoBehaviour
{
    [Header("文字组件")]
    [SerializeField] private TextMeshProUGUI text;

    [Header("闪烁设置")]
    [SerializeField] private BlinkMode blinkMode = BlinkMode.Smooth;
    [SerializeField] private float blinkSpeed = 1.5f;      // 闪烁速度
    [SerializeField] private float minAlpha = 0.2f;        // 最低透明度
    [SerializeField] private float maxAlpha = 1f;          // 最高透明度

    [Header("点击检测")]
    [SerializeField] private bool detectAnyKey = true;
    [SerializeField] private bool detectMouseClick = true;
    [SerializeField] private bool detectTouch = true;

    [Header("事件")]
    public UnityEvent onContinue;  // 点击后触发的事件

    private CanvasGroup canvasGroup;
    private float timer;

    public enum BlinkMode
    {
        Smooth,     // 平滑淡入淡出
        Hard        // 硬切（亮/灭）
    }

    void Awake()
    {
        // 自动获取组件
        if (text == null) text = GetComponent<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void Update()
    {
        HandleBlinking();
        HandleInput();
    }

    void HandleBlinking()
    {
        switch (blinkMode)
        {
            case BlinkMode.Smooth:
                // 正弦波平滑闪烁
                float alpha = Mathf.Lerp(minAlpha, maxAlpha,
                    (Mathf.Sin(Time.time * blinkSpeed * Mathf.PI) + 1f) * 0.5f);
                canvasGroup.alpha = alpha;
                break;

            case BlinkMode.Hard:
                // 定时硬切
                timer += Time.deltaTime;
                float period = 1f / blinkSpeed;
                canvasGroup.alpha = (timer % period) < (period * 0.5f) ? maxAlpha : minAlpha;
                break;
        }
    }

    void HandleInput()
    {
        bool triggered = false;

        if (detectAnyKey && Input.anyKeyDown)
            triggered = true;

        if (detectMouseClick && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
            triggered = true;

        if (detectTouch && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            triggered = true;

        if (triggered)
        {
            onContinue?.Invoke();
        }
    }

    // 公共方法：手动触发继续
    public void Continue()
    {
        onContinue?.Invoke();
    }

    // 公共方法：停止闪烁
    public void StopBlinking()
    {
        enabled = false;
        canvasGroup.alpha = maxAlpha;
    }
}