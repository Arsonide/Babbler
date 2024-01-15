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
    private const int PRIME = 97;
    
    private readonly List<PhoneticSound> _phoneticsToSpeak = new List<PhoneticSound>();
    private Coroutine _phoneticCoroutine;
    private PhoneticVoice _currentVoice;

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

            channel.setPitch(SpeechPitch * Utilities.GetRandomFloat(BabblerConfig.MinimumSyllablePitchVariance, BabblerConfig.MaximumSyllablePitchVariance));
            SetChannelPosition(SpeechSource.position, channel);
            FMODRegistry.TryUpdate();

            ActiveChannels.Add(channel);

            float delay = BabblerConfig.BaseSyllableDelay + Utilities.GetRandomFloat(BabblerConfig.MinimumSyllableDelayVariance, BabblerConfig.MaximumSyllableDelayVariance);
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
                minimumFrequency = BabblerConfig.PhoneticFrequencyMaleMinimum;
                maximumFrequency = BabblerConfig.PhoneticFrequencyMaleMaximum;
                break;
            case VoiceCategory.Female:
                minimumFrequency = BabblerConfig.PhoneticFrequencyFemaleMinimum;
                maximumFrequency = BabblerConfig.PhoneticFrequencyFemaleMaximum;
                break;
            default:
                minimumFrequency = BabblerConfig.PhoneticFrequencyNonBinaryMinimum;
                maximumFrequency = BabblerConfig.PhoneticFrequencyNonBinaryMaximum;
                break;
        }

        float frequency = minimumFrequency + (characteristics.Pitch * (maximumFrequency - minimumFrequency));
        return frequency / _currentVoice.Frequency;
    }
    
    public char PickMonosyllable(Human human)
    {
        int index = Math.Abs((human.seed.GetHashCode() * PRIME) % BabblerConfig.ValidMonosyllables.Length);
        return BabblerConfig.ValidMonosyllables[index];
    }
}