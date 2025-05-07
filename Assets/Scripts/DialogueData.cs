using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public LineData[] lines;
    [Header("Audio Trigger")]
    public bool isAudioTriggered;
    public string musicTrackName;
    public string sfxName;

    public Sprite background;
}

