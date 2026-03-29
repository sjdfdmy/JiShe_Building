using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModuleManager : MonoBehaviour
{
    private static ModuleManager instance;
    public static ModuleManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance=FindObjectOfType<ModuleManager>();
                if (instance == null)
                {
                    Debug.Log("No ModuleManager found in the scene!");
                }
            }
            return instance;
        }
    }

    public Button Exit;

    private void Awake()
    {
        Exit.onClick.AddListener(() => { InteractableManager.Instance.CloseInteractable(InteractableManager.InteractableType.Module); });
    }

    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
