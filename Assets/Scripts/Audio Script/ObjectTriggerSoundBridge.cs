// Attach this script to same GameObject as ObjectTrigger
public class ObjectTriggerSoundBridge : MonoBehaviour, UnityEngine.EventSystems.IPointerClickHandler
{
    private ObjectTrigger trigger;
    private AudioTriggerHelper audioHelper;

    void Start()
    {
        trigger = GetComponent<ObjectTrigger>();
        audioHelper = GetComponent<AudioTriggerHelper>();
    }

    public void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
    {
        audioHelper?.PlayTriggerAudio();
        trigger?.OnPointerClick(eventData);
    }
}
