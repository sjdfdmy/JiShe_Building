using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalentObj : MonoBehaviour,IInteractable
{
    public string GetInteractPrompt()
    {
        return "按 F 键进入能力培训界面";
    }
    public void OnInteract(PlayerMoveManager player)
    {
        if (player == null)
        {
            return;
        }
        InteractableManager.Instance.Interactable(InteractableManager.InteractableType.Talent);
    }
}
