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
    [Header("Audio")]
    public AudioClip typewriterSFX1;
    public AudioClip typewriterSFX2;
    public AudioClip carriageReturnSFX;
    [Range(0f, 1f)] public float typewriterVolume = 1f;
    [Range(0f, 1f)] public float carriageReturnVolume = 1f;
    public float typeDelay = 0.04f;
    public int charsPerSFX = 2;

    private AudioSource audioSource;


    void Start()
    {
        dialogueQueue = new Queue<LineData>();
        dialoguePanel.SetActive(false);
        choicesPanel.SetActive(false);
        audioSource = gameObject.AddComponent<AudioSource>();

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
        int charCount = 0;

        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            charCount++;

            if (charCount % charsPerSFX == 0)
            {
                PlayRandomTypewriterSFX();
            }

            yield return new WaitForSeconds(typeDelay);
        }

        isTyping = false;

        if (carriageReturnSFX != null)
        {
            audioSource.PlayOneShot(carriageReturnSFX, carriageReturnVolume);
        }

        if (currentDisplayedLine.hasChoices && currentDisplayedLine.choices.Length > 0)
        {
            waitingForChoice = true;
            ShowChoices(currentDisplayedLine.choices);
        }
    }
    void PlayRandomTypewriterSFX()
    {
        if (typewriterSFX1 == null || typewriterSFX2 == null)
            return;

        AudioClip selected = Random.value < 0.5f ? typewriterSFX1 : typewriterSFX2;
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(selected, typewriterVolume);
        audioSource.pitch = 1f;
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




