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
        Speaker = BabblerConfig.Mode == SpeechMode.Synthesis ? new SynthesisSpeaker() : new PhoneticSpeaker();
        
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