using UnityEngine;
using UnityEngine.UI;

public class MainMenuAchievementButton : MonoBehaviour
{
    public Button achievementButton;
    public GameObject redDot;
    public ManualPageManager galleryUI;

    private void Start()
    {
        if (achievementButton != null)
            achievementButton.onClick.AddListener(OnClick);

        if (AchievementManager.Instance != null)
            AchievementManager.Instance.OnNewAchievementChanged += UpdateRedDot;

        UpdateRedDot();
    }

    private void OnDestroy()
    {
        if (AchievementManager.Instance != null)
            AchievementManager.Instance.OnNewAchievementChanged -= UpdateRedDot;
    }

    private void UpdateRedDot()
    {
        if (redDot != null)
        {
            bool hasNew = AchievementManager.Instance != null && AchievementManager.Instance.HasNewAchievement();
            redDot.SetActive(hasNew);
        }
    }

    private void OnClick()
    {
        
        Debug.Log("点击成就按钮");
        if (galleryUI != null)
        {
            Debug.Log("调用 Open");
            galleryUI.Open();
        }
        else
        {
            Debug.LogError("galleryUI 为空，请在 Inspector 中把 PageManager 拖进来");
        }
    }
}