using UnityEngine;

public class DialogueAudioTrigger : MonoBehaviour
{
    public string musicTrackName;
    public string sfxOnStart;

    private bool hasPlayed = false;

    public void PlayTriggerAudio()
    {
        if (!hasPlayed)
        {
            if (!string.IsNullOrEmpty(musicTrackName))
                AudioManager.Instance.PlayMusic(musicTrackName);

            if (!string.IsNullOrEmpty(sfxOnStart))
                AudioManager.Instance.PlaySFX(sfxOnStart);

            hasPlayed = true;
        }
    }
}
