using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DialogueSystem : MonoBehaviour
{
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI dialogueText;
    public Image portraitImage;
    public GameObject dialoguePanel;
    public Image backgroundImage; // Reference to the background UI image


    [Header("Choices")]
    public GameObject choicesPanel;
    public GameObject choiceButtonPrefab;

    [Header("Audio")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float typewriterVolume = 1f;
    [Range(0f, 1f)] public float carriageReturnVolume = 1f;
    public AudioClip[] musicClips;
    public AudioClip typewriterSFX1;
    public AudioClip typewriterSFX2;
    public AudioClip carriageReturnSFX;
    private AudioSource musicSource;
    private Queue<LineData> dialogueQueue;
    private bool isDialogueActive = false;
    private bool waitingForChoice = false;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private LineData currentDisplayedLine;
    public float typeDelay = 0.04f;
    public int charsPerSFX = 2;

    private AudioSource audioSource;

    void Start()
    {
        dialogueQueue = new Queue<LineData>();
        dialoguePanel.SetActive(false);
        choicesPanel.SetActive(false);

        // Create internal music AudioSource
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void StartDialogue(DialogueData dialogueData)
    {
        if (dialogueData.background != null)
        {
            backgroundImage.sprite = dialogueData.background;
        }

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

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        currentDisplayedLine = dialogueQueue.Dequeue();

        if (currentDisplayedLine.triggerMusicSwitch && !string.IsNullOrEmpty(currentDisplayedLine.musicTrackName))
        {
            PlayMusicByName(currentDisplayedLine.musicTrackName);
        }

        characterNameText.text = currentDisplayedLine.characterName;
        portraitImage.sprite = currentDisplayedLine.portrait;

        typingCoroutine = StartCoroutine(TypeText(currentDisplayedLine.text));
    }

    void PlayMusicByName(string name)
    {
        AudioClip foundClip = System.Array.Find(musicClips, c => c.name == name);
        if (foundClip != null && musicSource.clip != foundClip)
        {
            musicSource.clip = foundClip;
            musicSource.Play();
        }
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
            yield return new WaitForSeconds(0.04f);
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

    void CompleteTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

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
    void PlayRandomTypewriterSFX()
    {
        if (typewriterSFX1 == null || typewriterSFX2 == null) return;

        AudioClip clip = Random.value < 0.5f ? typewriterSFX1 : typewriterSFX2;
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(clip, typewriterVolume);
        audioSource.pitch = 1f;
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        isDialogueActive = false;
        musicSource.Stop();
    }

    void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space) && !choicesPanel.activeSelf)
        {
            DisplayNextLine();
        }
    }
}
