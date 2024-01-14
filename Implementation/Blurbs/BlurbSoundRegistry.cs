using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Babbler.Implementation.Common;
using FMOD;

namespace Babbler.Implementation.Blurbs;

public static class BlurbSoundRegistry
{
    private static Dictionary<string, BlurbSound> Map = new Dictionary<string, BlurbSound>();

    public static void Initialize()
    {
        Map.Clear();
        string directory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException(), "Blurbs");
        
        // TODO set up blurb "voices" in subdirectories.
        foreach (string filePath in Directory.GetFiles(directory, "*.wav"))
        {
            string noExtension = Path.GetFileNameWithoutExtension(filePath);
            string[] split = noExtension.Split('_');

            if (split.Length != 2)
            {
                continue;
            }

            string phonetic = split[1].ToLowerInvariant();
            BlurbSound newBlurb = CreateBlurbSound(filePath, phonetic);
            
            // Space is used for all punctuation marks. Otherwise, the phonetic is the phonetic.
            if (phonetic.Contains("space"))
            {
                Map[" "] = newBlurb;
                Map[","] = newBlurb;
                Map["."] = newBlurb;
                Map["?"] = newBlurb;
                Map["!"] = newBlurb;
            }
            else
            {
                Map[phonetic] = newBlurb;
            }
        }
    }

    public static void Uninitialize()
    {
        foreach (KeyValuePair<string, BlurbSound> pair in Map)
        {
            if (pair.Value.Released)
            {
                continue;
            }

            pair.Value.Sound.release();
            pair.Value.Released = true;
        }
    }
    
    private static BlurbSound CreateBlurbSound(string filePath, string phonetic)
    {
        if (!FMODRegistry.TryCreateSound(filePath, MODE.DEFAULT | MODE._3D, out Sound sound))
        {
            return null;
        }
        
        sound.getLength(out uint length, TIMEUNIT.MS);
        
        BlurbSound newBlurb = new BlurbSound()
        {
            Phonetic = phonetic, FilePath = filePath, Sound = sound, Length = length / 1000f, Released = false,
        };

        return newBlurb;
    }

    public static bool TryGetBlurbSound(string phonetic, out BlurbSound result)
    {
        return Map.TryGetValue(phonetic, out result);
    }
}