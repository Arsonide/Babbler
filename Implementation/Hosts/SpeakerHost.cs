using UnityEngine;
using Babbler.Implementation.Common;
using Babbler.Implementation.Speakers;

namespace Babbler.Implementation.Hosts;

public class SpeakerHost : MonoBehaviour
{
    public SpeakerHostPool Pool;
    public BaseSpeaker Speaker;
    
    private bool _initialized;

    private void OnDestroy()
    {
        Uninitialize();
    }
    
    public void Initialize()
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;

        if (Pool.SpeakerType == SpeakerType.Speech)
        {
            switch (Pool.SpeechMode)
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
        }
        else
        {
            Speaker = new EmoteSpeaker();
        }
        
        Speaker.OnFinishedSpeaking -= OnFinishedSpeaking;
        Speaker.OnFinishedSpeaking += OnFinishedSpeaking;
        
        Speaker.InitializeSpeaker();
    }

    private void Uninitialize()
    {
        if (!_initialized)
        {
            return;
        }
        
        Speaker.OnFinishedSpeaking -= OnFinishedSpeaking;
        Speaker.UninitializeSpeaker();
        _initialized = false;
    }

    private void OnEnable()
    {
        if (Speaker != null)
        {
            Speaker.StopSpeaker();
        }
    }

    private void OnDisable()
    {
        if (Speaker != null)
        {
            Speaker.StopSpeaker();
        }
    }

    private void Update()
    {
        Speaker.UpdateSpeaker();
    }
    
    private void OnFinishedSpeaking()
    {
        Pool.ReleaseSpeakerHost(this);
    }
}