using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseObj : MonoBehaviour,IInteractable
{
    public string GetInteractPrompt()
    {
        return "按 F 键进入采购界面";
    }
    public void OnInteract(PlayerMoveManager player)
    {
        if (player == null)
        {
            return;
        }
        InteractableManager.Instance.Interactable(InteractableManager.InteractableType.NPC);
    }
}
