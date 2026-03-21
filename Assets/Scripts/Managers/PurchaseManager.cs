using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseManager : MonoBehaviour
{
    private static PurchaseManager instance;
    public static PurchaseManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance=FindObjectOfType<PurchaseManager>();
                if (instance == null)
                {
                    Debug.Log("No PurchaseManager found in the scene!");
                }
            }
            return instance;
        }
    }

    public Button Exit;

    private void Awake()
    {
        Exit.onClick.AddListener(() => { InteractableManager.Instance.CloseInteractable(InteractableManager.InteractableType.NPC); });
    }

    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
