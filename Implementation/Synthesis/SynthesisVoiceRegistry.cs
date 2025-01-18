#pragma warning disable CA1416

using System;
using System.Collections.Generic;
using System.Speech.Synthesis;
using BepInEx.Logging;
using Babbler.Implementation.Characteristics;
using Babbler.Implementation.Common;
using Babbler.Implementation.Config;

namespace Babbler.Implementation.Synthesis;

public static class SynthesisVoiceRegistry
{
    public static List<InstalledVoice> OneCoreVoices = null;
    
    private const int PRIME_VOICE = 37;
    
    private static List<string> MaleVoices = new List<string>();
    private static List<string> FemaleVoices = new List<string>();
    private static List<string> NonBinaryVoices = new List<string>();
    private static List<string> AllVoices = new List<string>();
    private static List<string> VoiceFilterInput = new List<string>();
    private static List<string> VoiceLocaleFilter = new List<string>();

    private static readonly List<string>[] VoicePriorities = new List<string>[4];
    
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

        SetupVoiceFilterInput();
        SetupVoiceLocaleFilter();
        SpeechSynthesizer synthesizer = new SpeechSynthesizer();

        try
        {
            OneCoreVoices = synthesizer.GetOneCoreVoices();

            if (OneCoreVoices != null)
            {
                synthesizer.AddVoices(OneCoreVoices);
            }
        }
        catch (Exception e)
        {
            Utilities.Log($"Exception encountered while SynthesisVoiceRegistry tried to get OneCore voices: {e.Message}", LogLevel.Debug);
        }

        foreach (InstalledVoice voice in synthesizer.GetInstalledVoices())
        {
            if (!PassesVoiceFilters(voice))
            {
                continue;
            }
            
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
                case VoiceGender.Neutral:
                case VoiceGender.NotSet:
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
        
        Utilities.Log($"SynthesisVoiceRegistry has initialized! Male Voices: {MaleVoices.Count}, Female Voices: {FemaleVoices.Count}, Non-Binary Voices: {NonBinaryVoices.Count}", LogLevel.Debug);

        // If we are in Synthesis mode but have no voices, revert to Phonetic mode. We ship with phonetic voices so we know it should work.
        if (AllVoices.Count <= 0 && BabblerConfig.Mode.Value == SpeechMode.Synthesis)
        {
            BabblerConfig.Mode.Value = SpeechMode.Phonetic;
            Utilities.Log("The plugin is configured for Synthesis but no voices are installed, reverting to Phonetic mode!", LogLevel.Error);
        }
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
        return characteristics.SelectDeterministicGenderedListElement(VoicePriorities, AllVoices, MaleVoices, FemaleVoices, NonBinaryVoices, PRIME_VOICE);
    }

    private static void SetupVoiceFilterInput()
    {
        VoiceFilterInput.Clear();
        string input = BabblerConfig.SynthesisVoiceFilterInput.Value.ToLowerInvariant();
        VoiceFilterInput.AddRange(input.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
    }

    private static void SetupVoiceLocaleFilter()
    {
        VoiceLocaleFilter.Clear();
        string input = BabblerConfig.SynthesisVoiceLocaleFilter.Value.ToLowerInvariant();
        VoiceLocaleFilter.AddRange(input.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
    }

    private static bool PassesVoiceFilters(InstalledVoice voice)
    {
        if (!PassesVoiceLocaleFilter(voice)) return false;
        return PassesVoiceFilterInput(voice);
    }

    private static bool PassesVoiceLocaleFilter(InstalledVoice voice)
    {
        foreach(string filter in VoiceLocaleFilter)
        {
            if (voice.VoiceInfo.Culture.Name.ToLowerInvariant().Contains(filter)) return true;
        }
        return false;
    }

    private static bool PassesVoiceFilterInput(InstalledVoice voice)
    {
        switch(BabblerConfig.SynthesisVoiceFilter.Value)
        {
            case SynthesisVoiceFilterType.Blacklist:
                foreach(string filter in VoiceFilterInput)
                {
                    if (voice.VoiceInfo.Name.ToLowerInvariant().Contains(filter))
                    {
                        return false;
                    }
                }

                return true;
            case SynthesisVoiceFilterType.Whitelist:
                foreach(string filter in VoiceFilterInput)
                {
                    if (voice.VoiceInfo.Name.ToLowerInvariant().Contains(filter))
                    {
                        return true;
                    }
                }

                return false;
            default:
                return true;
        }
    }
}

#pragma warning restore CA1416