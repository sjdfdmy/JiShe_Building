using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    private static StartManager instance;
    public static StartManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<StartManager>();
                if (instance == null)
                {
                    Debug.Log("No StartManager");
                }
            }
            return instance;
        }
    }
    public GameObject startall;
    public GameObject buttons;
    public GameObject title;
    public GameObject littletip;
    public Button startbutton;
    //public Button settingbutton;
    public Button exitbutton;

    private void Awake()
    {
        startbutton.onClick.AddListener(() =>
        {
            Scenemanager.Instance.LoadScene(Scenemanager.Scenes.Game);
        });
        //settingbutton.onClick.AddListener(() =>
        //{
            
        //});
        exitbutton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }

    void Start()
    {
        buttons.SetActive(false);
        title.SetActive(true);
        littletip.SetActive(true);
    }

    void Update()
    {
        
    }

    public void EnterStart()
    {
        startall.GetComponent<Animator>().SetTrigger("Start");
        littletip.SetActive(false);
    }

}
