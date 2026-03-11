using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("过渡参数")]
    [Tooltip("渐隐/渐显时长（秒）")]
    public float fadeDuration = 0.5f;
    
    private CanvasGroup fadeCanvasGroup;
    private bool isFading = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetupFadeCanvas();
    }
    
    private void SetupFadeCanvas()
    {
        GameObject canvasGO = new GameObject("TransitionCanvas");
        canvasGO.transform.SetParent(transform);

        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();
        
        GameObject panelGO = new GameObject("FadePanel");
        panelGO.transform.SetParent(canvasGO.transform, false);

        Image image = panelGO.AddComponent<Image>();
        image.color = Color.black;

        RectTransform rect = panelGO.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        
        fadeCanvasGroup = panelGO.AddComponent<CanvasGroup>();
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;
        fadeCanvasGroup.interactable = false;
    }
    
    public void LoadScene(string sceneName)
    {
        if (!isFading)
            StartCoroutine(TransitionCoroutine(sceneName));
    }
    
    public void LoadScene(int sceneIndex)
    {
        if (!isFading)
            StartCoroutine(TransitionCoroutine(sceneIndex));
    }

    private IEnumerator TransitionCoroutine(object sceneIdentifier)
    {
        isFading = true;
        fadeCanvasGroup.blocksRaycasts = true;
        
        yield return StartCoroutine(Fade(0f, 1f));
        
        if (sceneIdentifier is string sceneName)
            SceneManager.LoadScene(sceneName);
        else if (sceneIdentifier is int sceneIndex)
            SceneManager.LoadScene(sceneIndex);
        
        yield return null;
        
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
            t = Mathf.SmoothStep(0f, 1f, t);
            fadeCanvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, t);
            yield return null;
        }

        fadeCanvasGroup.alpha = toAlpha;
    }
}
