using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableManager : MonoBehaviour
{
    private static InteractableManager instance;
    public static InteractableManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance=FindAnyObjectByType<InteractableManager>();
                if (instance == null)
                {
                    Debug.Log("No InteractableManager found in the scene!");
                }
            }
            return instance;
        }
    }

    public enum InteractableType
    {
        Talent,
        Drag,
        Shop,
        Module,
        Quest,
        NPC
    }

    public PlayerMoveManager movemanager;
    public GameObject talent;
    public GameObject drag;
    public GameObject shop;
    public GameObject module;

    private void Start()
    {
        talent.SetActive(false);
        drag.SetActive(false);
        //shop.SetActive(false);
        module.SetActive(false);
    }

    public void Interactable(InteractableType type)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        movemanager.enabled = false;
        switch (type)
        {
            case InteractableType.Talent:
                talent.SetActive(true);
                break;
            case InteractableType.Drag:
                drag.SetActive(true);
                break;
            case InteractableType.Shop:
                shop.SetActive(true);
                break;
            case InteractableType.Module:
                movemanager.enabled = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                module.SetActive(true);
                break;
            case InteractableType.Quest:

                break;
            case InteractableType.NPC:

                break;
        }
    }

    public void CloseInteractable(InteractableType type)
    {
        movemanager.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        switch (type)
        {
            case InteractableType.Talent:
                talent.SetActive(false);
                break;
            case InteractableType.Drag:
                drag.SetActive(false);
                break;
            case InteractableType.Shop:
                shop.SetActive(false);
                break;
            case InteractableType.Module:
                module.SetActive(false);
                break;
            case InteractableType.Quest:

                break;
            case InteractableType.NPC:

                break;
        }
    }
}
