using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementItem : MonoBehaviour
{
    public Image backgroundImage;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public string achievementId;

    [Header("ÑÕÉ«")]
    public Color unlockedColor = new Color(1f, 0.9f, 0.6f);
    public Color lockedColor = new Color(0.3f, 0.3f, 0.3f);

    private AchievementData data;

    void Start()
    {
        if (!string.IsNullOrEmpty(achievementId) && AchievementManager.Instance != null)
        {
            data = AchievementManager.Instance.GetAchievementById(achievementId);
            if (data != null)
            {
                if (titleText != null)
                    titleText.text = data.title;
                if (descriptionText != null)
                    descriptionText.text = data.description;
                RefreshDisplay();
            }
        }
    }

    public void RefreshDisplay()
    {
        if (data == null) return;

        if (backgroundImage != null)
        {
            backgroundImage.color = data.isUnlocked ? unlockedColor : lockedColor;
        }

        if (titleText != null)
        {
            titleText.color = data.isUnlocked ? Color.white : new Color(0.6f, 0.6f, 0.6f);
        }

        if (descriptionText != null)
        {
            descriptionText.color = data.isUnlocked ? new Color(0.9f, 0.9f, 0.8f) : new Color(0.5f, 0.5f, 0.5f);
        }
    }

    public void Initialize(AchievementData ach)
    {
        data = ach;
        achievementId = ach.id;
        if (titleText != null)
            titleText.text = ach.title;
        if (descriptionText != null)
            descriptionText.text = ach.description;
        RefreshDisplay();
    }
}