using UnityEngine;
using Babbler.Implementation.Common;
using Babbler.Implementation.Config;

namespace Babbler.Implementation.Characteristics;

public struct VoiceCharacteristics
{
    private const int PRIME_DIVERSITY = 31;
    private const int PRIME_PITCH = 47;
    private const int PRIME_RATE = 89;

    public VoiceCategory Category;
    public float Pitch;
    public float Rate;

    private VoiceCharacteristics(VoiceCategory category, float pitch, float rate)
    {
        Category = category;
        Pitch = pitch;
        Rate = rate;
    }
    
    public static VoiceCharacteristics Create(Human human, bool maleVoiceAvailable, bool femaleVoiceAvailable, bool nonBinaryVoiceAvailable)
    {
        int hashCode = human.seed.GetHashCode();
        float diverseGenderScale = GetDiverseGenderScale(human, hashCode);
        VoiceCategory category = GetProfileInformation(diverseGenderScale, hashCode, maleVoiceAvailable, femaleVoiceAvailable, nonBinaryVoiceAvailable, out float pitchScalar, out float rateScalar);
        return new VoiceCharacteristics(category, pitchScalar, rateScalar);
    }

    private static float GetDiverseGenderScale(Human human, int hashCode)
    {
        float diversity = ((hashCode % PRIME_DIVERSITY) / (float)PRIME_DIVERSITY - 0.5f) * 2 * BabblerConfig.GenderDiversity.Value;
        return Mathf.Clamp(human.genderScale + diversity, 0, 1);
    }

    private static VoiceCategory GetProfileInformation(float diverseGenderScale, int hashCode, bool hasMaleVoices, bool hasFemaleVoices, bool hasNonBinaryVoices, out float pitchScalar, out float rateScalar)
    {
        rateScalar = Utilities.GetDeterministicFloat(hashCode, PRIME_RATE, 0f, 1f);

        if (diverseGenderScale < BabblerConfig.FemaleThreshold.Value && hasFemaleVoices)
        {
            pitchScalar = Mathf.InverseLerp(0f, BabblerConfig.FemaleThreshold.Value, diverseGenderScale);
            return VoiceCategory.Female;
        }
        
        if (diverseGenderScale > BabblerConfig.MaleThreshold.Value && hasMaleVoices)
        {
            pitchScalar = Mathf.InverseLerp(BabblerConfig.MaleThreshold.Value, 1f, diverseGenderScale);
            return VoiceCategory.Male;
        }

        if (hasNonBinaryVoices)
        {
            pitchScalar = Mathf.InverseLerp(BabblerConfig.FemaleThreshold.Value, BabblerConfig.MaleThreshold.Value, diverseGenderScale);
            return VoiceCategory.NonBinary;
        }

        pitchScalar = Utilities.GetDeterministicFloat(hashCode, PRIME_PITCH, 0f, 1f);
        return VoiceCategory.Any;
    }
}