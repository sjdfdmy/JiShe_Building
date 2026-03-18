using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDataManager : MonoBehaviour
{
    private static GameDataManager instance;
    public static GameDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance=FindObjectOfType<GameDataManager>();
                if (instance == null)
                {
                    Debug.Log("No GameDataManager found!");
                }
            }
            return instance;
        }
    }

    [Header("Persistence")]
    public Vector3 lastPlayerPosition;
    public string currentSceneName;
    
    [Header("���")]
    public Transform player;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {

    }


    void Update()
    {
        if (player != null)
        {
            lastPlayerPosition = player.position;
        }
        
        string activeScene = SceneManager.GetActiveScene().name;
        if (currentSceneName != activeScene)
        {
            currentSceneName = activeScene;
            Debug.Log($"Scene changed to: {currentSceneName}");
        }
    }
    
    public void SavePlayerData()
    {
        if (player != null)
        {
            lastPlayerPosition = player.position;
        }
        currentSceneName = SceneManager.GetActiveScene().name;
        
        PlayerPrefs.SetFloat("PlayerX", lastPlayerPosition.x);
        PlayerPrefs.SetFloat("PlayerY", lastPlayerPosition.y);
        PlayerPrefs.SetString("SavedScene", currentSceneName);
        PlayerPrefs.Save();
    }
}
