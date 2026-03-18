using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingIconManager : MonoBehaviour
{
    private static BuildingIconManager instance;
    public static BuildingIconManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance=FindObjectOfType<BuildingIconManager>();
                if (instance == null)
                {
                    Debug.Log("No BuildingIconManager found in the scene!");
                }
            }
            return instance;
        }
    }

    public GameObject icon;
    public Button[] buttons=new Button[5];
    public GameObject[] icons=new GameObject[5];

    private void Awake()
    {
        for(int i = 0; i < icons.Length; i++)
        {
            if (buttons[i] != null && icons[i] != null)
            {
                buttons[i].onClick.AddListener(() =>
                {
                    icon.SetActive(true);
                    for(int j = 0; j < icons.Length; j++)
                    {
                        if(icons[j] != null)
                            icons[j].SetActive(i==j);
                    }
                });
            }
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
