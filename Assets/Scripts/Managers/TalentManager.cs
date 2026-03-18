using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalentManager : MonoBehaviour
{
    private static TalentManager instance;
    public static TalentManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance=FindObjectOfType<TalentManager>();
                if (instance == null)
                {
                    Debug.Log("No TalentManager found in the scene!");
                }
            }
            return instance;
        }
    }

    public Button Exit;

    private void Awake()
    {
        Exit.onClick.AddListener(() => { InteractableManager.Instance.CloseInteractable(InteractableManager.InteractableType.Talent); });
    }

    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
