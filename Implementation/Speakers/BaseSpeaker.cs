using System;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using Babbler.Implementation.Common;
using Babbler.Implementation.Config;

namespace Babbler.Implementation.Speakers;

public abstract class BaseSpeaker
{
    public Action OnFinishedSpeaking;
    
    protected SpeechContext SpeechContext { get; private set; }
    protected Human SpeechPerson { get; private set; }
    protected Transform SpeechSource { get; private set; }
    protected float SpeechPitch { get; private set; }
    protected float SpeechVolume { get; private set; }

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

    public virtual void StartSpeaker(string speechInput, SpeechContext speechContext, Human speechPerson)
    {
        StopSpeaker();

        SpeechContext = speechContext;
        SpeechPerson = speechPerson;

        SpeechSource = CacheSpeechSource(speechContext, speechPerson);
        SpeechPitch = CacheSpeechPitch(speechPerson);
        SpeechVolume = CacheSpeechVolume(speechContext);
    }

    public virtual void UpdateSpeaker()
    {
        Vector3 position = SpeechSource.position;
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
            }
        }

        if (dirty)
        {
            FMODRegistry.System.update();
        }
    }

    protected virtual float CacheSpeechPitch(Human speechPerson)
    {
        return 1f;
    }

    protected virtual float CacheSpeechVolume(SpeechContext speechContext)
    {
        // TODO: Maybe this could be part of the channel group instead?
        switch (speechContext)
        {
            case SpeechContext.ConversationalSpeech:
                return BabblerConfig.ConversationalVolume;
            case SpeechContext.PhoneSpeech:
                return BabblerConfig.PhoneVolume;
            default:
                return BabblerConfig.OverheardVolume;
        }
    }

    private Transform CacheSpeechSource(SpeechContext speechContext, Human speechPerson)
    {
        Transform result = null;
        
        if (speechContext == SpeechContext.PhoneSpeech)
        {
            // These all appear to be valid at different times so search for everything.
            GameObject receiver = Player.Instance.interactingWith?.controller?.phoneReciever ??
                                  Player.Instance.phoneInteractable?.controller?.phoneReciever ??
                                  Player.Instance.answeringPhone?.interactable?.controller?.phoneReciever;

            result = receiver?.transform;
        }
        else
        {
            result = speechPerson?.lookAtThisTransform;
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
}