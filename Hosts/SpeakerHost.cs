using UnityEngine;

namespace Babbler;

public class SpeakerHost : MonoBehaviour
{
    public BaseSpeaker Speaker;

    private void Awake()
    {
        Speaker = new BabbleSpeaker();
        Speaker.OnFinishedSpeaking += OnFinishedSpeaking;
    }
    
    private void OnEnable()
    {
        Speaker.StopSpeaker();
    }

    private void OnDisable()
    {
        Speaker.StopSpeaker();
    }

    private void Update()
    {
        Speaker.UpdateSpeaker();
    }
    
    private void OnFinishedSpeaking()
    {
        SpeakerHostPool.ReleaseBabbler(this);
    }
}