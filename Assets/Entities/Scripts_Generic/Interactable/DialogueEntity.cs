using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueEntity : InteractableEntity
{
    [SerializeField]
    private TextAsset entityDialogue;

    public override void Interact()
    {
        DialogueManager.Instance.StartDialogue(entityDialogue);
        base.Interact();
    }
}
