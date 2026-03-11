using UnityEngine;
using UnityEngine.UI;
using System;

public class TalentNodeUI : MonoBehaviour
{
    public Image backgroundImage;
    public Image iconImage;
    public Button clickButton;

    public Color lockedColor = new Color(0.2f, 0.2f, 0.2f);
    public Color availableColor = new Color(0.4f, 0.3f, 0.2f);
    public Color unlockedColor = new Color(0.8f, 0.7f, 0.5f);

    public void Setup(Sprite icon, bool isUnlocked, bool isAvailable, Action onClick)
    {
        if (iconImage != null && icon != null)
            iconImage.sprite = icon;

        if (isUnlocked)
        {
            backgroundImage.color = unlockedColor;
            iconImage.color = Color.white;
        }
        else if (isAvailable)
        {
            backgroundImage.color = availableColor;
            iconImage.color = new Color(0.7f, 0.7f, 0.7f);
        }
        else
        {
            backgroundImage.color = lockedColor;
            iconImage.color = new Color(0.4f, 0.4f, 0.4f);
        }

        clickButton.interactable = true;
        clickButton.onClick.RemoveAllListeners();
        if (onClick != null)
            clickButton.onClick.AddListener(() => onClick());
    }
}