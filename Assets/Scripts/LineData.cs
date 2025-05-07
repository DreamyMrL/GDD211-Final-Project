using UnityEngine;

[System.Serializable]
public class LineData
{
    public string characterName;
    [TextArea(2, 5)] public string text;
    public Sprite portrait;

    public bool hasChoices;
    public DialogueChoice[] choices;

    [Header("Optional Audio")]
    public bool triggerMusicSwitch;
    public string musicTrackName;
}
