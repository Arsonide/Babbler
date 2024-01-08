using System.Collections.Generic;
using UnityEngine;

namespace Babbler;

public static class BabblerPool
{
    private static List<Babbler> Pool = new List<Babbler>();

    public static void Play(Human human, string babblerInput)
    {
        Babbler babbler = GetBabbler();
        babbler.StartBabbleRoutine(human, babblerInput);
    }
    
    private static Babbler GetBabbler()
    {
        Babbler babbler;
        int lastIndex = Pool.Count - 1;

        if (lastIndex >= 0)
        {
            babbler = Pool[lastIndex];
            Pool.RemoveAt(lastIndex);
            babbler.gameObject.SetActive(true);
        }
        else
        {
            GameObject go = new GameObject("Babbler");
            babbler = go.AddComponent<Babbler>();
        }
        
        return babbler;
    }

    public static void ReleaseBabbler(Babbler babbler)
    {
        babbler.gameObject.SetActive(false);
        Pool.Add(babbler);
    }
}