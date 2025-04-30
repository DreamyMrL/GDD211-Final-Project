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
    private bool waitingForChoice = false;

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private LineData currentDisplayedLine; // ADDED: track current line

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
        waitingForChoice = false;
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (waitingForChoice) return;

        if (isTyping)
        {
            CompleteTyping();
            return;
        }

        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        currentDisplayedLine = dialogueQueue.Dequeue();

        characterNameText.text = currentDisplayedLine.characterName;
        portraitImage.sprite = currentDisplayedLine.portrait;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(currentDisplayedLine.text));
    }

    IEnumerator TypeText(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.04f);
        }

        isTyping = false;

        // After text finishes typing, check if choices exist
        if (currentDisplayedLine.hasChoices && currentDisplayedLine.choices.Length > 0)
        {
            waitingForChoice = true;
            ShowChoices(currentDisplayedLine.choices);
        }
    }

    void CompleteTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.text = currentDisplayedLine.text;
        isTyping = false;

        if (currentDisplayedLine.hasChoices && currentDisplayedLine.choices.Length > 0)
        {
            waitingForChoice = true;
            ShowChoices(currentDisplayedLine.choices);
        }
    }

    void ShowChoices(DialogueChoice[] choices)
    {
        choicesPanel.SetActive(true);

        // Clear previous buttons
        foreach (Transform child in choicesPanel.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < choices.Length; i++)
        {
            DialogueChoice choice = choices[i];
            GameObject buttonObj = Instantiate(choiceButtonPrefab, choicesPanel.transform);
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = choice.choiceText;

            Button button = buttonObj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                choicesPanel.SetActive(false);
                waitingForChoice = false;

                if (choice.nextDialogue != null)
                {
                    StartDialogue(choice.nextDialogue);
                }
                else
                {
                    DisplayNextLine();
                }
            });
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
}





