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

    [Header("Choices")]
    public GameObject choicesPanel;
    public GameObject choiceButtonPrefab;

    private Queue<LineData> dialogueQueue;
    private bool isDialogueActive = false;



    void Start()
    {
        dialogueQueue = new Queue<LineData>();
        dialoguePanel.SetActive(false);
        choicesPanel.SetActive(false);
    }

    public void StartDialogue(DialogueData dialogueData)
    {
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

        if (currentLine.hasChoices && currentLine.choices.Length > 0)
        {
            ShowChoices(currentLine.choices);
        }
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
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space) && !choicesPanel.activeSelf)
        {
            DisplayNextLine();
        }
    }

    void ShowChoices(DialogueChoice[] choices)
    {
        choicesPanel.SetActive(true);

        // Clear existing buttons
        foreach (Transform child in choicesPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (DialogueChoice choice in choices)
        {
            GameObject buttonObj = Instantiate(choiceButtonPrefab, choicesPanel.transform);
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = choice.choiceText;

            Button button = buttonObj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                choicesPanel.SetActive(false);
                if (choice.nextDialogue != null)
                {
                    StartDialogue(choice.nextDialogue); // Branch to another dialogue
                }
                else
                {
                    DisplayNextLine(); // Continue current dialogue if no branch
                }
            });
        }
    }
}



