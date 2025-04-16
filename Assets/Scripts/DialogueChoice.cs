using UnityEngine;

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;
    public DialogueData nextDialogue; // Optional for branching
}

