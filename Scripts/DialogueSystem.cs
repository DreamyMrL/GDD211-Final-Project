using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI dialogueText;
    public Image portraitImage;
    public GameObject dialoguePanel;

    private Queue<LineData> dialogueQueue;
    private bool isDialogueActive = false;

    void Start()
    {
        dialogueQueue = new Queue<LineData>();
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(DialogueData dialogueData)
    {
        if (dialogueData == null || dialogueData.lines.Length == 0)
        {
            Debug.LogWarning("DialogueData is empty or null!");
            return;
        }

        dialogueQueue.Clear();

        foreach (LineData line in dialogueData.lines)
        {
            dialogueQueue.Enqueue(line);
        }

        dialoguePanel.SetActive(true);
        isDialogueActive = true;
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        LineData currentLine = dialogueQueue.Dequeue();

        characterNameText.text = currentLine.characterName;
        portraitImage.sprite = currentLine.portrait;

        StopAllCoroutines();
        StartCoroutine(TypeText(currentLine.text));
    }

    IEnumerator TypeText(string line)
    {
        dialogueText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.04f);
        }
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        isDialogueActive = false;
    }

    void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            DisplayNextLine();
        }
    }
}


