using System;
using System.Collections;
using System.Collections.Generic;
using FMOD;
using UnityEngine;

namespace Babbler;

public class BabbleSpeaker : BaseSpeaker
{
    private readonly List<PhoneticSound> _phoneticsToBabble = new List<PhoneticSound>();
    private Coroutine _babbleCoroutine;

    public override void StopSpeaker()
    {
        base.StopSpeaker();

        if (_babbleCoroutine != null)
        {
            UniverseLib.RuntimeHelper.StopCoroutine(_babbleCoroutine);
            _babbleCoroutine = null;
        }
    }

    public override void StartSpeaker(string speechInput, SpeechContext speechContext, Human speechPerson)
    {
        base.StartSpeaker(speechInput, speechContext, speechPerson);

        PopulatePhonetics(speechInput);
        _babbleCoroutine = UniverseLib.RuntimeHelper.StartCoroutine(BabbleRoutine());
    }
    
    private IEnumerator BabbleRoutine()
    {
        foreach (PhoneticSound phonetic in _phoneticsToBabble)
        {
            FMODReferences.System.playSound(phonetic.Sound, FMODReferences.GetChannelGroup(SpeechContext), false, out Channel channel);
            
            channel.setPitch(SpeechPitch);
            channel.setVolume(SpeechVolume);
        
            SetChannelPosition(SpeechSource.position, channel);
            FMODReferences.System.update();

            ActiveChannels.Add(channel);
            
            yield return new WaitForSeconds(Mathf.Max(0f, phonetic.Length - BabblerConfig.SyllableSpeed));
        }
        
        OnFinishedSpeaking?.Invoke();
    }
    
    private void PopulatePhonetics(string input)
    {
        _phoneticsToBabble.Clear();
        ReadOnlySpan<char> inputSpan = input.ToLowerInvariant().AsSpan();

        for (int i = 0; i < inputSpan.Length; ++i)
        {
            ReadOnlySpan<char> span = inputSpan.Slice(i, Math.Min(2, inputSpan.Length - i));
            string spanString = span.ToString();

            if (span.Length > 1 && PhoneticSoundDatabase.TryGetPhonetic(spanString, out PhoneticSound phonetic))
            {
                _phoneticsToBabble.Add(phonetic);
                i++;
            }
            else if (PhoneticSoundDatabase.TryGetPhonetic(spanString[0].ToString(), out phonetic))
            {
                _phoneticsToBabble.Add(phonetic);
            }
        }
    }

    protected override float CacheSpeechPitch(Human speechPerson)
    {
        float genderScale = speechPerson != null ? speechPerson.genderScale : 0.5f;
        return Mathf.Lerp(BabblerConfig.MinimumPitch, BabblerConfig.MaximumPitch, 1f - genderScale);
    }

    // TODO: Maybe this could be part of the channel group instead?
    protected override float CacheSpeechVolume(SpeechContext speechContext)
    {
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
}