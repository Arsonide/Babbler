using UnityEngine;
using Babbler.Implementation.Common;
using Babbler.Implementation.Config;

namespace Babbler.Implementation.Speakers;

public class DroningSpeaker : PhoneticSpeaker
{
    private const int PRIME_PHONEME = 97;

    protected override void ProcessSpeechInput(Human speechPerson, ref string speechInput)
    {
        base.ProcessSpeechInput(speechPerson, ref speechInput);
        
        char monosyllable = PickPhoneme(speechPerson);
        Utilities.GlobalStringBuilder.Clear();
        
        foreach (char c in speechInput)
        {
            Utilities.GlobalStringBuilder.Append(c != ' ' ? monosyllable : c);
        }

        speechInput = Utilities.GlobalStringBuilder.ToString();
    }
    
    private char PickPhoneme(Human human)
    {
        return BabblerConfig.DroningValidPhonemes.Value[Utilities.GetDeterministicInteger(CurrentHash, PRIME_PHONEME, 0, BabblerConfig.DroningValidPhonemes.Value.Length)];
    }
    
    protected override void CacheSpeechVarianceFactors()
    {
        if (Utilities.GetDeterministicFloat(CurrentHash, PRIME_DELAY_CHANCE, 0f, 1f) <= BabblerConfig.DroningChanceDelayVariance.Value)
        {
            CurrentDelayVarianceFactor = Utilities.GetDeterministicFloat(CurrentHash, PRIME_DELAY_FACTOR, 0f, 1f);
        }
        else
        {
            CurrentDelayVarianceFactor = -1f;
        }
        
        if (Utilities.GetDeterministicFloat(CurrentHash, PRIME_PITCH_CHANCE, 0f, 1f) <= BabblerConfig.DroningChancePitchVariance.Value)
        {
            CurrentPitchVarianceFactor = Utilities.GetDeterministicFloat(CurrentHash, PRIME_PITCH_FACTOR, 0f, 1f);
        }
        else
        {
            CurrentPitchVarianceFactor = -1f;
        }
    }

    protected override float GetPhonemeDelay()
    {
        float naturalDelay = BabblerConfig.DroningSpeechDelay.Value;

        if (CurrentDelayVarianceFactor < 0f)
        {
            return naturalDelay;
        }
        
        float targetDelay = BabblerConfig.DroningSpeechDelay.Value + Utilities.GetRandomFloat(BabblerConfig.DroningMinDelayVariance.Value, BabblerConfig.DroningMaxDelayVariance.Value);
        return Mathf.Lerp(naturalDelay, targetDelay, CurrentDelayVarianceFactor);
    }
    
    protected override float GetPhonemePitch()
    {
        float naturalPitch = SpeechPitch;

        if (CurrentPitchVarianceFactor < 0f)
        {
            return naturalPitch;
        }

        float targetPitch = naturalPitch * Utilities.GetRandomFloat(BabblerConfig.DroningMinPitchVariance.Value, BabblerConfig.DroningMaxPitchVariance.Value);
        return Mathf.Lerp(naturalPitch, targetPitch, CurrentPitchVarianceFactor);
    }
}