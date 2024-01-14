using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using Babbler.Implementation.Blurbs;
using Babbler.Implementation.Characteristics;
using Babbler.Implementation.Common;
using Babbler.Implementation.Config;

namespace Babbler.Implementation.Speakers;

public class BlurbSpeaker : BaseSpeaker
{
    private const int PRIME = 97;
    
    private readonly List<BlurbSound> _blurbsToSpeak = new List<BlurbSound>();
    private Coroutine _blurbCoroutine;

    public override void StopSpeaker()
    {
        base.StopSpeaker();

        if (_blurbCoroutine != null)
        {
            UniverseLib.RuntimeHelper.StopCoroutine(_blurbCoroutine);
            _blurbCoroutine = null;
        }
    }

    public override void StartSpeaker(string speechInput, SpeechContext speechContext, Human speechPerson)
    {
        base.StartSpeaker(speechInput, speechContext, speechPerson);

        if (BabblerConfig.UseMonosyllabicBlurbs)
        {
            speechInput = ProcessMonosyllabicBlurb(speechInput, speechPerson);
        }

        PopulateBlurbSounds(speechInput);
        _blurbCoroutine = UniverseLib.RuntimeHelper.StartCoroutine(BlurbRoutine());
    }

    private IEnumerator BlurbRoutine()
    {
        foreach (BlurbSound blurb in _blurbsToSpeak)
        {
            if (!FMODRegistry.TryPlaySound(blurb.Sound, FMODRegistry.GetChannelGroup(SpeechContext), out Channel channel))
            {
                continue;
            }
            
            channel.setPitch(SpeechPitch);
            SetChannelPosition(SpeechSource.position, channel);
            FMODRegistry.TryUpdate();

            ActiveChannels.Add(channel);
            yield return blurb.Yield;
        }
        
        OnFinishedSpeaking?.Invoke();
    }
    
    private string ProcessMonosyllabicBlurb(string speechInput, Human speechPerson)
    {
        char monosyllable = PickMonosyllable(speechPerson);
        Utilities.GlobalStringBuilder.Clear();
        
        foreach (char c in speechInput)
        {
            Utilities.GlobalStringBuilder.Append(char.IsLetter(c) ? monosyllable : c);
        }

        return Utilities.GlobalStringBuilder.ToString();
    }
    
    private void PopulateBlurbSounds(string speechInput)
    {
        _blurbsToSpeak.Clear();
        ReadOnlySpan<char> inputSpan = speechInput.ToLowerInvariant().AsSpan();

        for (int i = 0; i < inputSpan.Length; ++i)
        {
            ReadOnlySpan<char> span = inputSpan.Slice(i, Math.Min(2, inputSpan.Length - i));
            string spanString = span.ToString();

            if (span.Length > 1 && BlurbSoundRegistry.TryGetBlurbSound(spanString, out BlurbSound blurbSound))
            {
                _blurbsToSpeak.Add(blurbSound);
                i++;
            }
            else if (BlurbSoundRegistry.TryGetBlurbSound(spanString[0].ToString(), out blurbSound))
            {
                _blurbsToSpeak.Add(blurbSound);
            }
        }
    }

    protected override float CacheSpeechPitch(Human speechPerson)
    {
        // We're just blurbin' so we have all the voices.
        VoiceCharacteristics characteristics = VoiceCharacteristics.Create(speechPerson, true, true, true);
        
        switch (characteristics.Category)
        {
            case VoiceCategory.Male:
                return Mathf.Lerp(BabblerConfig.BlurbsPitchMaleMinimum, BabblerConfig.BlurbsPitchMaleMaximum, characteristics.Pitch);
            case VoiceCategory.Female:
                return Mathf.Lerp(BabblerConfig.BlurbsPitchFemaleMinimum, BabblerConfig.BlurbsPitchFemaleMaximum, characteristics.Pitch);
            default:
                return Mathf.Lerp(BabblerConfig.BlurbsPitchNonBinaryMinimum, BabblerConfig.BlurbsPitchNonBinaryMaximum, characteristics.Pitch);
        }
    }
    
    public char PickMonosyllable(Human human)
    {
        int index = Math.Abs((human.seed.GetHashCode() * PRIME) % BabblerConfig.ValidMonosyllables.Length);
        return BabblerConfig.ValidMonosyllables[index];
    }
}