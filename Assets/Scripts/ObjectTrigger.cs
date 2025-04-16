using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectTrigger : MonoBehaviour, IPointerClickHandler
{
    public DialogueSystem dialogueSystem;
    public DialogueData dialogueData;

    void Start()
    {
        if (dialogueSystem == null)
        {
            dialogueSystem = FindObjectOfType<DialogueSystem>();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (dialogueSystem != null && dialogueData != null)
        {
            dialogueSystem.StartDialogue(dialogueData);
        }
    }
}

