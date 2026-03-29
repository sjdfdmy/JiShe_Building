using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class AchievementPopup : MonoBehaviour
{
    public static AchievementPopup Instance;

    public GameObject popupPanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public float displayTime = 3f;

    [Header("动画设置")]
    public float appearDuration = 0.3f;   // 淡入时间
    public float disappearDuration = 0.3f; // 淡出时间

    private CanvasGroup canvasGroup;
    private Coroutine currentCoroutine;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
            canvasGroup = popupPanel.GetComponent<CanvasGroup>();

            // 如果没有CanvasGroup，添加一个
            if (canvasGroup == null)
                canvasGroup = popupPanel.AddComponent<CanvasGroup>();

            canvasGroup.alpha = 0;
        }
    }

    private void Start()
    {
        if (AchievementManager.Instance != null)
            AchievementManager.Instance.OnAchievementUnlocked += ShowPopup;
    }

    private void OnDestroy()
    {
        if (AchievementManager.Instance != null)
            AchievementManager.Instance.OnAchievementUnlocked -= ShowPopup;
    }

    private void ShowPopup(AchievementData ach)
    {
        if (titleText != null)
            titleText.text = ach.title;
        if (descriptionText != null)
            descriptionText.text = ach.description;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(PlayPopupAnimation());
    }

    private IEnumerator PlayPopupAnimation()
    {
        // 重置透明度
        canvasGroup.alpha = 0;
        popupPanel.SetActive(true);

        // 淡入
        float elapsed = 0;
        while (elapsed < appearDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / appearDuration;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t);
            yield return null;
        }
        canvasGroup.alpha = 1;

        // 等待显示时间
        yield return new WaitForSeconds(displayTime);

        // 淡出
        elapsed = 0;
        while (elapsed < disappearDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / disappearDuration;
            canvasGroup.alpha = Mathf.Lerp(1, 0, t);
            yield return null;
        }

        popupPanel.SetActive(false);
        canvasGroup.alpha = 0;
        currentCoroutine = null;
    }
}