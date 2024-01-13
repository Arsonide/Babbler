using System.Collections.Generic;
using UnityEngine;
using Babbler.Implementation.Common;

namespace Babbler.Implementation.Hosts;

public static class SpeakerHostPool
{
    private static List<SpeakerHost> Pool = new List<SpeakerHost>();

    public static void Play(string speechInput, SpeechContext speechContext, Human speechPerson)
    {
        SpeakerHost speakerHost = GetSpeakerHost();
        speakerHost.Speaker.StartSpeaker(speechInput, speechContext, speechPerson);
    }
    
    private static SpeakerHost GetSpeakerHost()
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

    public static void ReleaseSpeakerHost(SpeakerHost speakerHost)
    {
        speakerHost.gameObject.SetActive(false);
        Pool.Add(speakerHost);
    }
}