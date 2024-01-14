using UnityEngine;
using Babbler.Implementation.Config;

namespace Babbler.Implementation.Characteristics;

public struct VoiceCharacteristics
{
    private const int PRIME1 = 31;
    private const int PRIME2 = 47;
    
    public VoiceCategory Category;
    public float Pitch;

    private VoiceCharacteristics(VoiceCategory category, float pitch)
    {
        Category = category;
        Pitch = pitch;
    }
    
    public static VoiceCharacteristics Create(Human human, bool maleVoiceAvailable, bool femaleVoiceAvailable, bool nonBinaryVoiceAvailable)
    {
        int hashCode = human.seed.GetHashCode();
        float diverseGenderScale = GetDiverseGenderScale(human, hashCode);
        VoiceCategory category = GetProfileInformation(diverseGenderScale, hashCode, maleVoiceAvailable, femaleVoiceAvailable, nonBinaryVoiceAvailable, out float pitchScalar);
        return new VoiceCharacteristics(category, pitchScalar);
    }

    private static float GetDiverseGenderScale(Human human, int hashCode)
    {
        float diversity = ((hashCode % PRIME1) / (float)PRIME1 - 0.5f) * 2 * BabblerConfig.GenderDiversity;
        return Mathf.Clamp(human.genderScale + diversity, 0, 1);
    }

    private static VoiceCategory GetProfileInformation(float diverseGenderScale, int hashCode, bool hasMaleVoices, bool hasFemaleVoices, bool hasNonBinaryVoices, out float pitchScalar)
    {
        if (diverseGenderScale < BabblerConfig.FemaleThreshold && hasFemaleVoices)
        {
            pitchScalar = Mathf.InverseLerp(0f, BabblerConfig.FemaleThreshold, diverseGenderScale);
            return VoiceCategory.Female;
        }
        
        if (diverseGenderScale > BabblerConfig.MaleThreshold && hasMaleVoices)
        {
            pitchScalar = Mathf.InverseLerp(BabblerConfig.MaleThreshold, 1f, diverseGenderScale);
            return VoiceCategory.Male;
        }

        if (hasNonBinaryVoices)
        {
            pitchScalar = Mathf.InverseLerp(BabblerConfig.FemaleThreshold, BabblerConfig.MaleThreshold, diverseGenderScale);
            return VoiceCategory.NonBinary;
        }

        pitchScalar = (Mathf.Abs(hashCode * PRIME2) % PRIME2) / (float)PRIME2;
        return VoiceCategory.Any;
    }
}