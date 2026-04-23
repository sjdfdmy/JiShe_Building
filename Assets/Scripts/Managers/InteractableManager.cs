using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InteractableManager : MonoBehaviour
{
    private static InteractableManager instance;
    public static InteractableManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<InteractableManager>();
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
    public GameObject purchase;

    private void Start()
    {
        talent.SetActive(false);
        drag.SetActive(false);
        //shop.SetActive(false);
        module.SetActive(false);
        purchase.SetActive(false);
    }

    public void Interactable(InteractableType type)
    {
        Cursor.lockState = CursorLockMode.None;
        GameDataManager.Instance.cursor.SetActive(true);
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
                module.SetActive(true);
                break;
            case InteractableType.Quest:

                break;
            case InteractableType.NPC:
                purchase.SetActive(true);
                break;
        }
    }

    public void Interactable(InteractableType type, GameObject loading)
    {
        UnityEvent fun = new UnityEvent();
        fun.AddListener(() => Interactable(type));
        TransitionManager.Instance.Loading(loading, 1.2f,fun);
        movemanager.enabled = false;
    }

    public void CloseInteractable(InteractableType type)
    {
        movemanager.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        GameDataManager.Instance.cursor.SetActive(false);
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
                purchase.SetActive(false);
                break;
        }
    }
}
