using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FMOD;
using BepInEx.Logging;
using Babbler.Implementation.Common;

namespace Babbler.Implementation.Phonetic;

public static class PhoneticSoundRegistry
{
    private static Dictionary<string, PhoneticSound> Map = new Dictionary<string, PhoneticSound>();

    public static void Initialize()
    {
        Map.Clear();
        string directory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException(), "Phonemes");
        
        // TODO set up phonetic "voices" in subdirectories.
        foreach (string filePath in Directory.GetFiles(directory, "*.wav"))
        {
            string noExtension = Path.GetFileNameWithoutExtension(filePath);
            string[] split = noExtension.Split('_');

            if (split.Length != 2)
            {
                continue;
            }

            string phonetic = split[1].ToLowerInvariant();
            PhoneticSound newPhonetic = CreatePhoneticSound(filePath, phonetic);
            
            // Space is used for all punctuation marks. Otherwise, the phonetic is the phonetic.
            if (phonetic.Contains("space"))
            {
                Map[" "] = newPhonetic;
                Map[","] = newPhonetic;
                Map["."] = newPhonetic;
                Map["?"] = newPhonetic;
                Map["!"] = newPhonetic;
            }
            else
            {
                Map[phonetic] = newPhonetic;
            }
        }
        
        Utilities.Log($"PhoneticSoundRegistry has initialized! Syllables: {Map.Count}", LogLevel.Debug);
    }

    public static void Uninitialize()
    {
        foreach (KeyValuePair<string, PhoneticSound> pair in Map)
        {
            if (pair.Value.Released)
            {
                continue;
            }

            pair.Value.Sound.release();
            pair.Value.Released = true;
        }
        
        Utilities.Log("PhoneticSoundRegistry has uninitialized!", LogLevel.Debug);
    }
    
    private static PhoneticSound CreatePhoneticSound(string filePath, string phonetic)
    {
        if (!FMODRegistry.TryCreateSound(filePath, MODE.DEFAULT | MODE._3D, out Sound sound))
        {
            return null;
        }
        
        sound.getLength(out uint length, TIMEUNIT.MS);
        float floatLength = length / 1000f;
        
        PhoneticSound newPhonetic = new PhoneticSound()
        {
            Phonetic = phonetic,
            FilePath = filePath,
            Sound = sound,
            Length = floatLength,
            Released = false,
        };

        return newPhonetic;
    }

    public static bool TryGetPhoneticSound(string phonetic, out PhoneticSound result)
    {
        return Map.TryGetValue(phonetic, out result);
    }
}