using System.Collections;
using UnityEngine;
using FMOD;
using Babbler.Implementation.Characteristics;
using Babbler.Implementation.Common;
using Babbler.Implementation.Config;
using Babbler.Implementation.Emotes;

namespace Babbler.Implementation.Speakers;

public class EmoteSpeaker : BaseSpeaker, IDelayableSpeaker
{
    private static float _emotesAllowedTimestamp = -1f;
    
    private EmoteSound _emoteToPlay;
    private Coroutine _emotePlayCoroutine;
    private float _delay;
    
    public override void StopSpeaker()
    {
        base.StopSpeaker();

        if (_emotePlayCoroutine != null)
        {
            UniverseLib.RuntimeHelper.StopCoroutine(_emotePlayCoroutine);
            _emotePlayCoroutine = null;
        }

        _delay = 0f;
    }

    public override void StartSpeaker(string speechInput, SoundContext soundContext, Human speechPerson)
    {
        base.StartSpeaker(speechInput, soundContext, speechPerson);

        if (_emoteToPlay == null)
        {
            OnFinishedSpeaking?.Invoke();
            return;
        }

        _emotePlayCoroutine = UniverseLib.RuntimeHelper.StartCoroutine(EmotePlayRoutine());
    }

    private IEnumerator EmotePlayRoutine()
    {
        while (_delay > 0f)
        {
            yield return null;
            _delay -= Time.deltaTime;
        }
        
        while (Time.realtimeSinceStartup < _emotesAllowedTimestamp)
        {
            yield return null;
        }
        
        if (!FMODRegistry.TryPlaySound(_emoteToPlay.Sound, FMODRegistry.GetChannelGroup(SoundContext), out Channel channel))
        {
            OnFinishedSpeaking?.Invoke();
            yield break;
        }

        float minStagger = BabblerConfig.EmotesMinStagger.Value;
        float maxStagger = BabblerConfig.EmotesMaxStagger.Value;
        float staggerDuration = (Utilities.GlobalRandom.NextSingle() * (maxStagger - minStagger) + minStagger);
        _emotesAllowedTimestamp = Time.realtimeSinceStartup + staggerDuration;
        
        channel.setPitch(BabblerConfig.EmotesUsePitchShifts.Value ? SpeechPitch : 1f);
        SetChannelPosition(SpeechSource.position, channel);
        FMODRegistry.TryUpdate();

        ActiveChannels.Add(channel);

        float expiration = Time.realtimeSinceStartup + _emoteToPlay.Length + 0.2f;

        while (Time.realtimeSinceStartup < expiration)
        {
            SetChannelPosition(SpeechSource.position, channel);
            FMODRegistry.TryUpdate();
            yield return null;
        }
        
        OnFinishedSpeaking?.Invoke();
    }

    protected override float CacheSpeechPitch(Human speechPerson, string speechInput)
    {
        // While we have a human, use this as an opportunity to choose the SpeechSynthesis voice.
        _emoteToPlay = EmoteSoundRegistry.GetEmote(speechInput, speechPerson, out VoiceCharacteristics characteristics);
        
        float minimumFrequency;
        float maximumFrequency;
        
        switch (characteristics.Category)
        {
            case VoiceCategory.Male:
                minimumFrequency = BabblerConfig.EmotesMinFrequencyMale.Value;
                maximumFrequency = BabblerConfig.EmotesMaxFrequencyMale.Value;
                break;
            case VoiceCategory.Female:
                minimumFrequency = BabblerConfig.EmotesMinFrequencyFemale.Value;
                maximumFrequency = BabblerConfig.EmotesMaxFrequencyFemale.Value;
                break;
            case VoiceCategory.NonBinary:
            default:
                minimumFrequency = BabblerConfig.EmotesMinFrequencyNonBinary.Value;
                maximumFrequency = BabblerConfig.EmotesMaxFrequencyNonBinary.Value;
                break;
        }

        float frequency = minimumFrequency + (characteristics.Pitch * (maximumFrequency - minimumFrequency));
        return frequency / _emoteToPlay.Frequency;
    }

    public void InitializeDelay(float delay)
    {
        _delay = delay;
    }
}