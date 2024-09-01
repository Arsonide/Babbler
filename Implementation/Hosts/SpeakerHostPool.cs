using System.Collections.Generic;
using UnityEngine;
using BepInEx.Logging;
using Babbler.Implementation.Common;
using Babbler.Implementation.Config;
using Babbler.Implementation.Speakers;

namespace Babbler.Implementation.Hosts;

public class SpeakerHostPool
{
    public static SpeakerHostPool Speech;
    public static SpeakerHostPool Emotes;

    public static void InitializePools()
    {
        Speech = new SpeakerHostPool(SpeakerType.Speech, BabblerConfig.Mode.Value);
        
        // The speech mode doesn't matter here because it isn't speech.
        Emotes = new SpeakerHostPool(SpeakerType.Emote, SpeechMode.Phonetic);
    }

    public static void UninitializePools()
    {
        Speech.CleanupSpeakerHosts();
        Emotes.CleanupSpeakerHosts();
    }

    public SpeakerType SpeakerType { get; private set; }
    public SpeechMode SpeechMode { get; private set; }

    private List<SpeakerHost> AvailableHosts = new List<SpeakerHost>();
    private List<SpeakerHost> AllHosts = new List<SpeakerHost>();
    
    private SpeakerHostPool(SpeakerType speakerType, SpeechMode speechMode)
    {
        SpeakerType = speakerType;
        SpeechMode = speechMode;
    }
    
    public void Play(string speechInput, SoundContext soundContext, Human speechPerson, float delay = 0f)
    {
        SpeakerHost speakerHost = GetSpeakerHost();
        
        if (delay > 0f && speakerHost.Speaker is IDelayableSpeaker delayableSpeaker)
        {
            delayableSpeaker.InitializeDelay(delay);
        }
        
        speakerHost.Speaker.StartSpeaker(speechInput, soundContext, speechPerson);
    }
    
    private SpeakerHost GetSpeakerHost()
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

            // BepInEx complains if we pass the pool in with Initialize...
            speakerHost.Pool = this;
            speakerHost.Initialize();
            
            Log($"Created speaker host, current count is {AllHosts.Count}.");
        }
        
        return speakerHost;
    }

    public void ReleaseSpeakerHost(SpeakerHost speakerHost)
    {
        speakerHost.gameObject.SetActive(false);
        AvailableHosts.Add(speakerHost);
    }

    private void CleanupSpeakerHosts()
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
        
        Log("Cleaned up all speaker hosts.");
    }

    private void Log(string log)
    {
#pragma warning disable CS0162

        if (!Utilities.DEBUG_BUILD)
        {
            return;
        }
        
        string type = "Emote";
        
        if (SpeakerType != SpeakerType.Emote)
        {
            type = SpeechMode.ToString();
        }
        
        Utilities.Log($"SpeakerHostPool ({type}): {log}", LogLevel.Debug);
        
#pragma warning restore CS0162
    }
}