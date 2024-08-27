using System;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using Babbler.Implementation.Common;
using BepInEx.Logging;

namespace Babbler.Implementation.Speakers;

public abstract class BaseSpeaker
{
    public Action OnFinishedSpeaking;
    
    protected SoundContext SoundContext { get; private set; }
    protected Human SpeechPerson { get; private set; }
    protected Transform SpeechSource { get; private set; }
    protected float SpeechPitch { get; private set; }

    protected readonly List<Channel> ActiveChannels = new List<Channel>();
    
    public virtual void InitializeSpeaker()
    {
        
    }

    public virtual void UninitializeSpeaker()
    {
        
    }
    
    public virtual void StopSpeaker()
    {
        ActiveChannels.Clear();
    }

    public virtual void StartSpeaker(string speechInput, SoundContext soundContext, Human speechPerson)
    {
        StopSpeaker();

        SoundContext = soundContext;
        SpeechPerson = speechPerson;

        SpeechSource = CacheSpeechSource(soundContext, speechPerson);
        SpeechPitch = CacheSpeechPitch(speechPerson, speechInput);
    }

    public virtual void UpdateSpeaker()
    {
        Vector3 position = Vector3.zero;

        if (SpeechSource != null)
        {
            position = SpeechSource.position;
        }
        else
        {
            Utilities.Log("Babbler speaker had a null SpeechSource, which should not happen!", LogLevel.Debug);
        }
        
        bool dirty = false;
        
        for (int i = ActiveChannels.Count - 1; i >= 0; --i)
        {
            Channel channel = ActiveChannels[i];
            
            if (channel.isPlaying(out bool isPlaying) == RESULT.OK && isPlaying)
            {
                SetChannelPosition(position, channel);
                dirty = true;
            }
            else
            {
                ActiveChannels.RemoveAt(i);
                
                if (ActiveChannels.Count <= 0)
                {
                    OnLastChannelFinished();
                }
            }
        }

        if (dirty)
        {
            FMODRegistry.TryUpdate();
        }
    }

    protected virtual float CacheSpeechPitch(Human speechPerson, string speechInput)
    {
        return 1f;
    }

    private Transform CacheSpeechSource(SoundContext soundContext, Human speechPerson)
    {
        Transform result;

        switch (soundContext)
        {
            case SoundContext.PhoneSpeech:
            case SoundContext.PhoneShout:
                // These all appear to be valid at different times so search for everything.
                GameObject receiver = Player.Instance.interactingWith?.controller?.phoneReciever ??
                                      Player.Instance.phoneInteractable?.controller?.phoneReciever ??
                                      Player.Instance.answeringPhone?.interactable?.controller?.phoneReciever;

                result = receiver?.transform;
                break;
            default:
                result = speechPerson?.lookAtThisTransform;
                break;
        }

        // Fallback in case something weird happens, which does happen.
        if (result == null)
        {
            result = Player.Instance?.transform;
        }

        return result;
    }
    
    protected void SetChannelPosition(Vector3 position, Channel channel)
    {
        VECTOR pos = new VECTOR
        {
            x = position.x,
            y = position.y,
            z = position.z,
        };
                
        VECTOR vel = new VECTOR
        {
            x = 0f,
            y = 0f,
            z = 0f,
        };

        channel.set3DAttributes(ref pos, ref vel);
    }

    protected virtual void OnLastChannelFinished()
    {
        
    }
}