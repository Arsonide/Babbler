using UnityEngine;
using Babbler.Implementation.Common;
using Babbler.Implementation.Config;
using Babbler.Implementation.Speakers;

namespace Babbler.Implementation.Hosts;

public class SpeakerHost : MonoBehaviour
{
    public BaseSpeaker Speaker;

    private void Awake()
    {
        switch (BabblerConfig.Mode)
        {
            case SpeechMode.Synthesis:
                Speaker = new SynthesisSpeaker();
                break;
            case SpeechMode.Phonetic:
                Speaker = new PhoneticSpeaker();
                break;
            case SpeechMode.Droning:
                Speaker = new DroningSpeaker();
                break;
            default:
                Speaker = new PhoneticSpeaker();
                break;
        }
        
        Speaker.OnFinishedSpeaking -= OnFinishedSpeaking;
        Speaker.OnFinishedSpeaking += OnFinishedSpeaking;
        
        Speaker.InitializeSpeaker();
    }

    private void OnDestroy()
    {
        Speaker.OnFinishedSpeaking -= OnFinishedSpeaking;
        Speaker.UninitializeSpeaker();
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
        SpeakerHostPool.ReleaseSpeakerHost(this);
    }
}