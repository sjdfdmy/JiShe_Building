using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GuidanceManager : MonoBehaviour
{
    private static GuidanceManager instance;
    public static GuidanceManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance=FindObjectOfType<GuidanceManager>();
                if (instance == null)
                {
                    Debug.Log("No GuidanceManager found!");
                }
            }
            return instance;
        }
    }
    public List<CreateGuide> guide;
    public int nowid=0;
    // UI
    public GameObject guidance;

    void Start()
    {

    }


    void Update()
    {
        guidance.SetActive(Scenemanager.Instance.nowscene != Scenemanager.Scenes.Start);
    }

    void Init()
    {
        nowid = 0;
    }

    void Next()
    {

    }
    
}
