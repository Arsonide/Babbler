using System.Collections.Generic;
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
    public float GenderScale;
    public float Pitch;
    public float Rate;
    public int Hash;

    private VoiceCharacteristics(VoiceCategory category, float genderScale, float pitch, float rate, int hash)
    {
        Category = category;
        GenderScale = genderScale;
        Pitch = pitch;
        Rate = rate;
        Hash = hash;
    }
    
    public static VoiceCharacteristics Create(Human human, bool maleVoiceAvailable, bool femaleVoiceAvailable, bool nonBinaryVoiceAvailable)
    {
        int hashCode = Utilities.GetDeterministicStringHash(human.seed);
        float diverseGenderScale = GetDiverseGenderScale(human, hashCode);
        VoiceCategory category = GetProfileInformation(diverseGenderScale, hashCode, maleVoiceAvailable, femaleVoiceAvailable, nonBinaryVoiceAvailable, out float pitchScalar, out float rateScalar);
        return new VoiceCharacteristics(category, diverseGenderScale, pitchScalar, rateScalar, hashCode);
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

    private void SortPriorityArray<T>(List<T>[] priorityArray, List<T> allList, List<T> maleList, List<T> femaleList, List<T> nonBinaryList)
    {
        switch (Category)
        {
            case VoiceCategory.Male:
                priorityArray[0] = maleList;
                priorityArray[1] = nonBinaryList;
                priorityArray[2] = femaleList;
                priorityArray[3] = allList;
                break;
            case VoiceCategory.Female:
                priorityArray[0] = femaleList;
                priorityArray[1] = nonBinaryList;
                priorityArray[2] = maleList;
                priorityArray[3] = allList;
                break;
            case VoiceCategory.NonBinary:
                bool masculine = GenderScale > 0.5f;
                priorityArray[0] = nonBinaryList;

                if (masculine)
                {
                    priorityArray[1] = maleList;
                    priorityArray[2] = femaleList;
                }
                else
                {
                    priorityArray[1] = femaleList;
                    priorityArray[2] = maleList;
                }

                priorityArray[3] = allList;
                break;
            default:
                priorityArray[0] = allList;
                priorityArray[1] = allList;
                priorityArray[2] = allList;
                priorityArray[3] = allList;
                break;
        }
    }
                                          
    public T SelectDeterministicGenderedListElement<T>(List<T>[] priorityArray, List<T> allList, List<T> maleList, List<T> femaleList, List<T> nonBinaryList, int prime)
    {
        SortPriorityArray(priorityArray, allList, maleList, femaleList, nonBinaryList);
        
        foreach(IList<T> list in priorityArray)
        {
            if (list.Count > 0)
            {
                return list[Utilities.GetDeterministicInteger(Hash, prime, 0, list.Count)];
            }
        }

        return default(T);
    }
    
    public T SelectRandomGenderedListElement<T>(List<T>[] priorityArray, List<T> allList, List<T> maleList, List<T> femaleList, List<T> nonBinaryList)
    {
        SortPriorityArray(priorityArray, allList, maleList, femaleList, nonBinaryList);
        
        foreach(IList<T> list in priorityArray)
        {
            if (list.Count > 0)
            {
                return list[Utilities.GlobalRandom.Next(0, list.Count)];
            }
        }

        return default(T);
    }
}