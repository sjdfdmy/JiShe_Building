using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 场景过渡管理器 — 单例，跨场景持久化。
/// 使用方式：SceneTransitionManager.Instance.LoadScene("SceneName");
/// 过渡效果：当前场景渐隐为黑色 → 加载新场景 → 新场景从黑色渐显。
/// </summary>
public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("过渡参数")]
    [Tooltip("渐隐/渐显时长（秒）")]
    public float fadeDuration = 0.5f;

    // 全屏黑色遮罩
    private CanvasGroup fadeCanvasGroup;
    private bool isFading = false;

    void Awake()
    {
        // 单例：已存在则销毁重复实例
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetupFadeCanvas();
    }

    /// <summary>
    /// 动态创建全屏黑色遮罩 Canvas（不需要在场景中手动配置）。
    /// </summary>
    private void SetupFadeCanvas()
    {
        // 创建专用 Canvas
        GameObject canvasGO = new GameObject("TransitionCanvas");
        canvasGO.transform.SetParent(transform);

        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; // 始终渲染在最上层

        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // 创建全屏黑色 Image
        GameObject panelGO = new GameObject("FadePanel");
        panelGO.transform.SetParent(canvasGO.transform, false);

        Image image = panelGO.AddComponent<Image>();
        image.color = Color.black;

        RectTransform rect = panelGO.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        // CanvasGroup 用于控制整体透明度
        fadeCanvasGroup = panelGO.AddComponent<CanvasGroup>();
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;
        fadeCanvasGroup.interactable = false;
    }

    /// <summary>
    /// 加载场景（带淡入淡出过渡效果）。
    /// 当前场景渐隐为黑色，加载新场景，新场景再从黑色渐显。
    /// </summary>
    /// <param name="sceneName">目标场景名称</param>
    public void LoadScene(string sceneName)
    {
        if (!isFading)
            StartCoroutine(TransitionCoroutine(sceneName));
    }

    /// <summary>
    /// 通过场景索引加载场景（带过渡效果）。
    /// </summary>
    /// <param name="sceneIndex">目标场景在 Build Settings 中的索引</param>
    public void LoadScene(int sceneIndex)
    {
        if (!isFading)
            StartCoroutine(TransitionCoroutine(sceneIndex));
    }

    private IEnumerator TransitionCoroutine(object sceneIdentifier)
    {
        isFading = true;
        fadeCanvasGroup.blocksRaycasts = true;

        // 渐隐：当前场景 → 黑色
        yield return StartCoroutine(Fade(0f, 1f));

        // 加载目标场景
        if (sceneIdentifier is string sceneName)
            SceneManager.LoadScene(sceneName);
        else if (sceneIdentifier is int sceneIndex)
            SceneManager.LoadScene(sceneIndex);

        // 等待一帧确保新场景已初始化
        yield return null;

        // 渐显：黑色 → 新场景
        yield return StartCoroutine(Fade(1f, 0f));

        fadeCanvasGroup.blocksRaycasts = false;
        isFading = false;
    }

    private IEnumerator Fade(float fromAlpha, float toAlpha)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            // 使用 SmoothStep 让过渡更自然
            t = Mathf.SmoothStep(0f, 1f, t);
            fadeCanvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, t);
            yield return null;
        }

        fadeCanvasGroup.alpha = toAlpha;
    }
}
