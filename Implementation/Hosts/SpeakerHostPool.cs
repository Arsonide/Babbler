using System.Collections.Generic;
using UnityEngine;
using Babbler.Implementation.Common;

namespace Babbler.Implementation.Hosts;

public static class SpeakerHostPool
{
    private static List<SpeakerHost> AvailableHosts = new List<SpeakerHost>();
    private static List<SpeakerHost> AllHosts = new List<SpeakerHost>();
    
    public static void Play(string speechInput, SpeechContext speechContext, Human speechPerson)
    {
        SpeakerHost speakerHost = GetSpeakerHost();
        speakerHost.Speaker.StartSpeaker(speechInput, speechContext, speechPerson);
    }
    
    private static SpeakerHost GetSpeakerHost()
    {
        SpeakerHost speakerHost;
        int lastIndex = AvailableHosts.Count - 1;

        if (lastIndex >= 0)
        {
            speakerHost = AvailableHosts[lastIndex];
            AvailableHosts.RemoveAt(lastIndex);
            speakerHost.gameObject.SetActive(true);
        }
        else
        {
            GameObject go = new GameObject("BabblerSpeakerHost");
            go.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            speakerHost = go.AddComponent<SpeakerHost>();
            Object.DontDestroyOnLoad(go);
            AllHosts.Add(speakerHost);
        }
        
        return speakerHost;
    }

    public static void ReleaseSpeakerHost(SpeakerHost speakerHost)
    {
        speakerHost.gameObject.SetActive(false);
        AvailableHosts.Add(speakerHost);
    }

    public static void CleanupSpeakerHosts()
    {
        for (int i = AllHosts.Count - 1; i >= 0; --i)
        {
            SpeakerHost speakerHost = AllHosts[i];

            if (speakerHost.gameObject.activeSelf)
            {
                speakerHost.gameObject.SetActive(false);
            }

            // This will uninitialize their BaseSpeakers, which will dispose of resources.
            Object.DestroyImmediate(speakerHost.gameObject);
        }
    }
}