using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx.Logging;
using Babbler.Implementation.Characteristics;
using Babbler.Implementation.Common;

namespace Babbler.Implementation.Phonetic;

public static class PhoneticVoiceRegistry
{
    private const int PRIME_VOICE = 211;
    
    private static readonly List<PhoneticVoice> Voices = new List<PhoneticVoice>();
    
    public static void Initialize()
    {
        Voices.Clear();
        
        string directory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException(), "Phonemes");

        foreach (string subdirectory in Directory.GetDirectories(directory))
        {
            PhoneticVoice voice = new PhoneticVoice();
            voice.Initialize(subdirectory);
            Voices.Add(voice);
        }
        
        Utilities.Log($"SynthesisVoiceRegistry has initialized! Voices: {Voices.Count}", LogLevel.Debug);
    }

    public static void Uninitialize()
    {
        foreach (PhoneticVoice voice in Voices)
        {
            voice.Uninitialize();
        }
    }
    
    public static PhoneticVoice GetVoice(Human human, out VoiceCharacteristics characteristics)
    {
        characteristics = VoiceCharacteristics.Create(human, true, true, true);

        // Trying to avoid instantiating a System.Random, so we do some math.
        return Voices[Utilities.GetDeterministicInteger(characteristics.Hash, PRIME_VOICE, 0, Voices.Count)];
    }
}