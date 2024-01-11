using System.Collections.Generic;
using UnityEngine;

namespace Babbler;

public static class SpeakerHostPool
{
    private static List<SpeakerHost> Pool = new List<SpeakerHost>();

    public static void Play(string speechInput, SpeechContext speechContext, Human speechPerson)
    {
        SpeakerHost speakerHost = GetSpeaker();
        speakerHost.Speaker.StartSpeaker(speechInput, speechContext, speechPerson);
    }
    
    private static SpeakerHost GetSpeaker()
    {
        SpeakerHost speakerHost;
        int lastIndex = Pool.Count - 1;

        if (lastIndex >= 0)
        {
            speakerHost = Pool[lastIndex];
            Pool.RemoveAt(lastIndex);
            speakerHost.gameObject.SetActive(true);
        }
        else
        {
            GameObject go = new GameObject("BabblerSpeakerHost");
            go.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            speakerHost = go.AddComponent<SpeakerHost>();
        }
        
        return speakerHost;
    }

    public static void ReleaseBabbler(SpeakerHost speakerHost)
    {
        speakerHost.gameObject.SetActive(false);
        Pool.Add(speakerHost);
    }
}