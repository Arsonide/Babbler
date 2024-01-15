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
        return BabblerConfig.DroningValidPhonemes[Utilities.GetDeterministicInteger(human.seed.GetHashCode(), PRIME_PHONEME, 0, BabblerConfig.DroningValidPhonemes.Length)];
    }
    
    protected override void CacheSpeechVarianceFactors(Human speechPerson)
    {
        int hash = speechPerson.seed.GetHashCode();

        if (Utilities.GetDeterministicFloat(hash, PRIME_DELAY_CHANCE, 0f, 1f) <= BabblerConfig.DroningChanceDelayVariance)
        {
            CurrentDelayVarianceFactor = Utilities.GetDeterministicFloat(hash, PRIME_DELAY_FACTOR, 0f, 1f);
        }
        else
        {
            CurrentDelayVarianceFactor = -1f;
        }
        
        if (Utilities.GetDeterministicFloat(hash, PRIME_PITCH_CHANCE, 0f, 1f) <= BabblerConfig.DroningChancePitchVariance)
        {
            CurrentPitchVarianceFactor = Utilities.GetDeterministicFloat(hash, PRIME_PITCH_FACTOR, 0f, 1f);
        }
        else
        {
            CurrentPitchVarianceFactor = -1f;
        }
    }

    protected override float GetPhonemeDelay()
    {
        float naturalDelay = BabblerConfig.DroningSpeechDelay;

        if (CurrentDelayVarianceFactor < 0f)
        {
            return naturalDelay;
        }
        
        float targetDelay = BabblerConfig.DroningSpeechDelay + Utilities.GetRandomFloat(BabblerConfig.DroningMinDelayVariance, BabblerConfig.DroningMaxDelayVariance);
        return Mathf.Lerp(naturalDelay, targetDelay, CurrentDelayVarianceFactor);
    }
    
    protected override float GetPhonemePitch()
    {
        float naturalPitch = SpeechPitch;

        if (CurrentPitchVarianceFactor < 0f)
        {
            return naturalPitch;
        }

        float targetPitch = naturalPitch * Utilities.GetRandomFloat(BabblerConfig.DroningMinPitchVariance, BabblerConfig.DroningMaxPitchVariance);
        return Mathf.Lerp(naturalPitch, targetPitch, CurrentPitchVarianceFactor);
    }
}