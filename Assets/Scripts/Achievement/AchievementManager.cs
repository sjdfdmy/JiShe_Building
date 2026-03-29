using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }

    public List<AchievementData> achievements = new List<AchievementData>();

    private const string UNLOCK_PREFIX = "ACH_";
    private const string NEW_PREFIX = "NEW_";

    public System.Action<AchievementData> OnAchievementUnlocked;
    public System.Action OnNewAchievementChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAllAchievements();
    }

    private void LoadAllAchievements()
    {
        foreach (var ach in achievements)
        {
            string key = UNLOCK_PREFIX + ach.id;
            ach.isUnlocked = PlayerPrefs.GetInt(key, 0) == 1;

            string newKey = NEW_PREFIX + ach.id;
            ach.hasNew = PlayerPrefs.GetInt(newKey, 0) == 1;
        }
    }

    private void SaveAchievement(AchievementData ach)
    {
        string key = UNLOCK_PREFIX + ach.id;
        PlayerPrefs.SetInt(key, ach.isUnlocked ? 1 : 0);

        string newKey = NEW_PREFIX + ach.id;
        PlayerPrefs.SetInt(newKey, ach.hasNew ? 1 : 0);

        PlayerPrefs.Save();
    }

    public bool UnlockAchievement(string achievementId)
    {
        AchievementData ach = achievements.Find(a => a.id == achievementId);
        if (ach == null || ach.isUnlocked) return false;

        ach.isUnlocked = true;
        ach.hasNew = true;
        SaveAchievement(ach);

        OnAchievementUnlocked?.Invoke(ach);
        OnNewAchievementChanged?.Invoke();

        return true;
    }

    public AchievementData GetAchievementById(string id)
    {
        return achievements.Find(a => a.id == id);
    }

    public List<AchievementData> GetAllAchievements()
    {
        return achievements;
    }

    public bool HasNewAchievement()
    {
        foreach (var ach in achievements)
        {
            if (ach.isUnlocked && ach.hasNew)
                return true;
        }
        return false;
    }

    public void ClearAllNewFlags()
    {
        bool changed = false;
        foreach (var ach in achievements)
        {
            if (ach.hasNew)
            {
                ach.hasNew = false;
                SaveAchievement(ach);
                changed = true;
            }
        }
        if (changed)
        {
            OnNewAchievementChanged?.Invoke();
        }
    }
}