using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ManualPageManager : MonoBehaviour
{
    [Header("页面")]
    public GameObject page1;
    public GameObject page2;
    public GameObject page3;

    [Header("按钮")]
    public Button leftButton;
    public Button rightButton;
    public Button closeButton;

    [Header("指示点")]
    public Image dot1;
    public Image dot2;
    public Image dot3;

    [Header("点颜色")]
    public Color activeColor = Color.black;
    public Color inactiveColor = Color.gray;

    [Header("成就面板")]
    public GameObject galleryPanel;

    [Header("滑动特效")]
    public float slideDuration = 0.3f;
    public float slideDistance = 800f;

    private int currentPage = 0;
    private GameObject[] pages;
    private Image[] dots;
    private bool isAnimating = false;
    private Coroutine currentAnimation = null;
    private bool isInitialized = false;

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (isInitialized) return;

        pages = new GameObject[] { page1, page2, page3 };
        dots = new Image[] { dot1, dot2, dot3 };

        if (leftButton != null)
            leftButton.onClick.AddListener(PreviousPage);
        if (rightButton != null)
            rightButton.onClick.AddListener(NextPage);
        if (closeButton != null)
            closeButton.onClick.AddListener(Close);

        AddDotClickEvents();

        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] != null)
            {
                RectTransform rect = pages[i].GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.anchoredPosition = Vector2.zero;
                }
                pages[i].SetActive(i == 0);
            }
        }

        currentPage = 0;
        UpdateDots(0);
        UpdateButtons();

        isInitialized = true;
    }

    private void AddDotClickEvents()
    {
        Image[] dotImages = new Image[] { dot1, dot2, dot3 };

        for (int i = 0; i < dotImages.Length; i++)
        {
            int pageIndex = i;
            if (dotImages[i] != null)
            {
                Button btn = dotImages[i].GetComponent<Button>();
                if (btn == null)
                    btn = dotImages[i].gameObject.AddComponent<Button>();

                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => GoToPage(pageIndex));
            }
        }
    }

    private void OnEnable()
    {
        // 如果还没初始化，先初始化
        if (!isInitialized)
        {
            Initialize();
        }

        // 现在安全调用
        SafeClearNewFlags();
        SafeRefreshAllCards();

        if (pages != null && currentPage >= 0 && currentPage < pages.Length && pages[currentPage] != null)
        {
            RectTransform rect = pages[currentPage].GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = Vector2.zero;
            }
        }
    }

    private void SafeClearNewFlags()
    {
        try
        {
            var instance = AchievementManager.Instance;
            if (instance != null)
            {
                instance.ClearAllNewFlags();
            }
        }
        catch
        {
            // 忽略所有错误
        }
    }

    private void SafeRefreshAllCards()
    {
        try
        {
            var allCards = FindObjectsOfType<AchievementItem>(true);
            if (allCards != null)
            {
                foreach (var card in allCards)
                {
                    if (card != null)
                    {
                        card.RefreshDisplay();
                    }
                }
            }
        }
        catch
        {
            // 忽略所有错误
        }
    }

    public void GoToPage(int pageIndex)
    {
        if (isAnimating) return;
        if (pages == null || pageIndex < 0 || pageIndex >= pages.Length) return;
        if (pageIndex == currentPage) return;

        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        currentAnimation = StartCoroutine(SlideAnimation(pageIndex));
    }

    IEnumerator SlideAnimation(int targetPage)
    {
        isAnimating = true;

        int direction = targetPage > currentPage ? -1 : 1;

        RectTransform currentRect = pages[currentPage].GetComponent<RectTransform>();
        RectTransform targetRect = pages[targetPage].GetComponent<RectTransform>();

        if (currentRect == null || targetRect == null)
        {
            isAnimating = false;
            yield break;
        }

        pages[targetPage].SetActive(true);
        targetRect.anchoredPosition = new Vector2(slideDistance * -direction, 0);

        float time = 0;
        Vector2 currentStart = currentRect.anchoredPosition;
        Vector2 targetStart = targetRect.anchoredPosition;

        while (time < slideDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / slideDuration);
            float easeOut = 1 - (1 - t) * (1 - t);

            currentRect.anchoredPosition = currentStart + new Vector2(slideDistance * direction * easeOut, 0);
            targetRect.anchoredPosition = targetStart + new Vector2(slideDistance * direction * easeOut, 0);

            yield return null;
        }

        currentRect.anchoredPosition = currentStart + new Vector2(slideDistance * direction, 0);
        targetRect.anchoredPosition = targetStart + new Vector2(slideDistance * direction, 0);

        pages[currentPage].SetActive(false);

        currentRect.anchoredPosition = Vector2.zero;
        targetRect.anchoredPosition = Vector2.zero;

        currentPage = targetPage;
        UpdateDots(currentPage);
        UpdateButtons();

        isAnimating = false;
        currentAnimation = null;
    }

    void UpdateDots(int pageIndex)
    {
        if (dots == null) return;
        for (int i = 0; i < dots.Length; i++)
        {
            if (dots[i] != null)
            {
                dots[i].color = (i == pageIndex) ? activeColor : inactiveColor;
            }
        }
    }

    void UpdateButtons()
    {
        if (leftButton != null)
            leftButton.interactable = currentPage > 0;
        if (rightButton != null)
            rightButton.interactable = currentPage < pages.Length - 1;
    }

    void PreviousPage()
    {
        if (currentPage > 0 && !isAnimating)
            GoToPage(currentPage - 1);
    }

    void NextPage()
    {
        if (currentPage < pages.Length - 1 && !isAnimating)
            GoToPage(currentPage + 1);
    }

    public void Close()
    {
        if (galleryPanel != null)
            galleryPanel.SetActive(false);
    }

    public void Open()
    {
        if (galleryPanel != null)
            galleryPanel.SetActive(true);
    }
}