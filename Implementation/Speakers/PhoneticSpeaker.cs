using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using Babbler.Implementation.Phonetic;
using Babbler.Implementation.Characteristics;
using Babbler.Implementation.Common;
using Babbler.Implementation.Config;

namespace Babbler.Implementation.Speakers;

public class PhoneticSpeaker : BaseSpeaker
{
    protected const int PRIME_DELAY_CHANCE = 401;
    protected const int PRIME_PITCH_CHANCE = 409;
    protected const int PRIME_DELAY_FACTOR = 419;
    protected const int PRIME_PITCH_FACTOR = 421;
    
    private readonly List<PhoneticSound> _phoneticsToSpeak = new List<PhoneticSound>();
    private Coroutine _phoneticCoroutine;
    private PhoneticVoice _currentVoice;

    protected int CurrentHash;

    protected float CurrentDelayVarianceFactor;
    protected float CurrentPitchVarianceFactor;
    
    public override void StopSpeaker()
    {
        base.StopSpeaker();

        if (_phoneticCoroutine != null)
        {
            UniverseLib.RuntimeHelper.StopCoroutine(_phoneticCoroutine);
            _phoneticCoroutine = null;
        }
    }

    public override void StartSpeaker(string speechInput, SpeechContext speechContext, Human speechPerson)
    {
        base.StartSpeaker(speechInput, speechContext, speechPerson);

        CurrentHash = Utilities.GetDeterministicStringHash(speechPerson.seed);
        
        CacheSpeechVarianceFactors();
        ProcessSpeechInput(speechPerson, ref speechInput);
        PopulatePhoneticSounds(speechInput);
        _phoneticCoroutine = UniverseLib.RuntimeHelper.StartCoroutine(PhoneticRoutine());
    }

    private IEnumerator PhoneticRoutine()
    {
        foreach (PhoneticSound phoneme in _phoneticsToSpeak)
        {
            if (!FMODRegistry.TryPlaySound(phoneme.Sound, FMODRegistry.GetChannelGroup(SpeechContext), out Channel channel))
            {
                continue;
            }

            channel.setPitch(GetPhonemePitch());
            SetChannelPosition(SpeechSource.position, channel);
            FMODRegistry.TryUpdate();

            ActiveChannels.Add(channel);

            float delay = GetPhonemeDelay();
            float syllableExpiration = Time.realtimeSinceStartup + phoneme.Length + delay;

            while (Time.realtimeSinceStartup < syllableExpiration)
            {
                yield return null;
            }
        }
        
        OnFinishedSpeaking?.Invoke();
    }

    protected virtual void ProcessSpeechInput(Human speechPerson, ref string speechInput)
    {
        // Do nothing, this is for DroningSpeaker.
    }

    private void PopulatePhoneticSounds(string speechInput)
    {
        _phoneticsToSpeak.Clear();
        ReadOnlySpan<char> inputSpan = speechInput.ToLowerInvariant().AsSpan();

        for (int i = 0; i < inputSpan.Length; ++i)
        {
            ReadOnlySpan<char> span = inputSpan.Slice(i, Math.Min(2, inputSpan.Length - i));
            string spanString = span.ToString();

            if (span.Length > 1 && _currentVoice.TryGetPhoneticSound(spanString, out PhoneticSound phoneticSound))
            {
                _phoneticsToSpeak.Add(phoneticSound);
                i++;
            }
            else if (_currentVoice.TryGetPhoneticSound(spanString[0].ToString(), out phoneticSound))
            {
                _phoneticsToSpeak.Add(phoneticSound);
            }
        }
    }

    protected override float CacheSpeechPitch(Human speechPerson)
    {
        // While we have a human, use this as an opportunity to choose the SpeechSynthesis voice.
        _currentVoice = PhoneticVoiceRegistry.GetVoice(speechPerson, out VoiceCharacteristics characteristics);
        
        float minimumFrequency;
        float maximumFrequency;
        
        switch (characteristics.Category)
        {
            case VoiceCategory.Male:
                minimumFrequency = BabblerConfig.PhoneticMinFrequencyMale.Value;
                maximumFrequency = BabblerConfig.PhoneticMaxFrequencyMale.Value;
                break;
            case VoiceCategory.Female:
                minimumFrequency = BabblerConfig.PhoneticMinFrequencyFemale.Value;
                maximumFrequency = BabblerConfig.PhoneticMaxFrequencyFemale.Value;
                break;
            case VoiceCategory.NonBinary:
            default:
                minimumFrequency = BabblerConfig.PhoneticMinFrequencyNonBinary.Value;
                maximumFrequency = BabblerConfig.PhoneticMaxFrequencyNonBinary.Value;
                break;
        }

        float frequency = minimumFrequency + (characteristics.Pitch * (maximumFrequency - minimumFrequency));
        return frequency / _currentVoice.Frequency;
    }

    protected virtual void CacheSpeechVarianceFactors()
    {
        if (Utilities.GetDeterministicFloat(CurrentHash, PRIME_DELAY_CHANCE, 0f, 1f) <= BabblerConfig.PhoneticChanceDelayVariance.Value)
        {
            CurrentDelayVarianceFactor = Utilities.GetDeterministicFloat(CurrentHash, PRIME_DELAY_FACTOR, 0f, 1f);
        }
        else
        {
            CurrentDelayVarianceFactor = -1f;
        }
        
        if (Utilities.GetDeterministicFloat(CurrentHash, PRIME_PITCH_CHANCE, 0f, 1f) <= BabblerConfig.PhoneticChancePitchVariance.Value)
        {
            CurrentPitchVarianceFactor = Utilities.GetDeterministicFloat(CurrentHash, PRIME_PITCH_FACTOR, 0f, 1f);
        }
        else
        {
            CurrentPitchVarianceFactor = -1f;
        }
    }

    protected virtual float GetPhonemeDelay()
    {
        float naturalDelay = BabblerConfig.PhoneticSpeechDelay.Value;

        if (CurrentDelayVarianceFactor < 0f)
        {
            return naturalDelay;
        }
        
        float targetDelay = BabblerConfig.PhoneticSpeechDelay.Value + Utilities.GetRandomFloat(BabblerConfig.PhoneticMinDelayVariance.Value, BabblerConfig.PhoneticMaxDelayVariance.Value);
        return Mathf.Lerp(naturalDelay, targetDelay, CurrentDelayVarianceFactor);
    }
    
    protected virtual float GetPhonemePitch()
    {
        float naturalPitch = SpeechPitch;

        if (CurrentPitchVarianceFactor < 0f)
        {
            return naturalPitch;
        }

        float targetPitch = naturalPitch * Utilities.GetRandomFloat(BabblerConfig.PhoneticMinPitchVariance.Value, BabblerConfig.PhoneticMaxPitchVariance.Value);
        return Mathf.Lerp(naturalPitch, targetPitch, CurrentPitchVarianceFactor);
    }
}