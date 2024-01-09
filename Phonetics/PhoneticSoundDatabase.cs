using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FMOD;

namespace Babbler;

public static class PhoneticSoundDatabase
{
    private static Dictionary<string, PhoneticSound> Map = new Dictionary<string, PhoneticSound>();

    public static void Initialize()
    {
        Map.Clear();
        string directory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException(), "sounds");
        
        foreach (string filePath in Directory.GetFiles(directory, "*.wav"))
        {
            string noExtension = Path.GetFileNameWithoutExtension(filePath);
            string[] split = noExtension.Split('_');

            if (split.Length != 2)
            {
                continue;
            }

            string phonetic = split[1].ToLowerInvariant();
            PhoneticSound newPhonetic = CreatePhonetic(filePath, phonetic);
            
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
    }
    
    private static PhoneticSound CreatePhonetic(string filePath, string phonetic)
    {
        RESULT result = FMODUnity.RuntimeManager.CoreSystem.createSound(filePath, MODE.DEFAULT | MODE._3D, out Sound sound);

        if (result != RESULT.OK)
        {
            return null;
        }
        
        sound.getLength(out uint length, TIMEUNIT.MS);
        
        PhoneticSound newPhonetic = new PhoneticSound()
        {
            Phonetic = phonetic, FilePath = filePath, Sound = sound, Length = length / 1000f, Released = false,
        };

        return newPhonetic;
    }

    public static bool TryGetPhonetic(string phonetic, out PhoneticSound result)
    {
        return Map.TryGetValue(phonetic, out result);
    }
}