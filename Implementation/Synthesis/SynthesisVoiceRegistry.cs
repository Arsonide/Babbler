#pragma warning disable CA1416

using System.Collections.Generic;
using System.Speech.Synthesis;
using UnityEngine;
using Babbler.Implementation.Characteristics;

namespace Babbler.Implementation.Synthesis;

public static class SynthesisVoiceRegistry
{
    private const int PRIME = 37;
    
    private static List<string> MaleVoices = new List<string>();
    private static List<string> FemaleVoices = new List<string>();
    private static List<string> NonBinaryVoices = new List<string>();
    private static List<string> AllVoices = new List<string>();

    private static bool HasMaleVoices;
    private static bool HasFemaleVoices;
    private static bool HasNonBinaryVoices;
    private static bool HasAnyVoices;
    
    public static void Initialize()
    {
        // This needs to be called during Plugin.Load, not in the main menu like FMOD, so we can kick start SpeechSynthesis.
        // It initializes our voices, categorizes them, and then plays a silent sound, which, if we do not do, the first time we use the API it will crash.
        // Yes, yes, you read that right.
        MaleVoices.Clear();
        FemaleVoices.Clear();
        NonBinaryVoices.Clear();
        AllVoices.Clear();
        
        SpeechSynthesizer synthesizer = new SpeechSynthesizer();

        foreach (InstalledVoice voice in synthesizer.GetInstalledVoices())
        {
            VoiceInfo voiceInfo = voice.VoiceInfo;
            string voiceName = voiceInfo.Name;
            VoiceGender voiceGender = voiceInfo.Gender;

            AllVoices.Add(voiceName);
            
            switch (voiceGender)
            {
                case VoiceGender.Male:
                    MaleVoices.Add(voiceName);
                    break;
                case VoiceGender.Female:
                    FemaleVoices.Add(voiceName);
                    break;
                default:
                    NonBinaryVoices.Add(voiceName);
                    break;
            }
        }

        HasMaleVoices = MaleVoices.Count > 0;
        HasFemaleVoices = FemaleVoices.Count > 0;
        HasNonBinaryVoices = NonBinaryVoices.Count > 0;
        HasAnyVoices = AllVoices.Count > 0;
        
        synthesizer.SpeakCompleted -= OnSpeakCompleted;
        synthesizer.SpeakCompleted += OnSpeakCompleted;

        synthesizer.SetOutputToNull();
        synthesizer.SpeakAsync("Initialize");
    }

    private static void OnSpeakCompleted(object sender, SpeakCompletedEventArgs e)
    {
        if (!(sender is SpeechSynthesizer synthesizer))
        {
            return;
        }
        
        synthesizer.SpeakCompleted -= OnSpeakCompleted;
        synthesizer.Dispose();
    }
    
    public static string GetVoice(Human human, out VoiceCharacteristics characteristics)
    {
        characteristics = VoiceCharacteristics.Create(human, HasMaleVoices, HasFemaleVoices, HasNonBinaryVoices);
        List<string> voices;

        switch (characteristics.Category)
        {
            case VoiceCategory.Male:
                voices = MaleVoices;
                break;
            case VoiceCategory.Female:
                voices = FemaleVoices;
                break;
            case VoiceCategory.NonBinary:
                voices = NonBinaryVoices;
                break;
            default:
                voices = AllVoices;
                break;
        }
        
        // Trying to avoid instantiating a System.Random, so we do some math.
        return voices[Mathf.Abs(human.seed.GetHashCode() * PRIME) % voices.Count];
    }
}

#pragma warning restore CA1416