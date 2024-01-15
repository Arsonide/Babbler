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

        if (BabblerConfig.UseMonosyllabicPhonetics)
        {
            speechInput = ProcessMonosyllabicPhonetics(speechInput, speechPerson);
        }

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
    
    private string ProcessMonosyllabicPhonetics(string speechInput, Human speechPerson)
    {
        char monosyllable = PickMonosyllable(speechPerson);
        Utilities.GlobalStringBuilder.Clear();
        
        foreach (char c in speechInput)
        {
            Utilities.GlobalStringBuilder.Append(char.IsLetter(c) ? monosyllable : c);
        }

        return Utilities.GlobalStringBuilder.ToString();
    }
    
    private void PopulatePhoneticSounds(string speechInput)
    {
        _phoneticsToSpeak.Clear();
        ReadOnlySpan<char> inputSpan = speechInput.ToLowerInvariant().AsSpan();

        for (int i = 0; i < inputSpan.Length; ++i)
        {
            ReadOnlySpan<char> span = inputSpan.Slice(i, Math.Min(2, inputSpan.Length - i));
            string spanString = span.ToString();

            if (span.Length > 1 && PhoneticSoundRegistry.TryGetPhoneticSound(spanString, out PhoneticSound phoneticSound))
            {
                _phoneticsToSpeak.Add(phoneticSound);
                i++;
            }
            else if (PhoneticSoundRegistry.TryGetPhoneticSound(spanString[0].ToString(), out phoneticSound))
            {
                _phoneticsToSpeak.Add(phoneticSound);
            }
        }
    }

    protected override float CacheSpeechPitch(Human speechPerson)
    {
        // TODO Voices are only really relevant to Synthesis until we add them to Phonetic mode.
        VoiceCharacteristics characteristics = VoiceCharacteristics.Create(speechPerson, true, true, true);
        
        switch (characteristics.Category)
        {
            case VoiceCategory.Male:
                return Mathf.Lerp(BabblerConfig.PhoneticPitchMaleMinimum, BabblerConfig.PhoneticPitchMaleMaximum, characteristics.Pitch);
            case VoiceCategory.Female:
                return Mathf.Lerp(BabblerConfig.PhoneticPitchFemaleMinimum, BabblerConfig.PhoneticPitchFemaleMaximum, characteristics.Pitch);
            default:
                return Mathf.Lerp(BabblerConfig.PhoneticPitchNonBinaryMinimum, BabblerConfig.PhoneticPitchNonBinaryMaximum, characteristics.Pitch);
        }
    }
    
    public char PickMonosyllable(Human human)
    {
        int index = Math.Abs((human.seed.GetHashCode() * PRIME) % BabblerConfig.ValidMonosyllables.Length);
        return BabblerConfig.ValidMonosyllables[index];
    }
}