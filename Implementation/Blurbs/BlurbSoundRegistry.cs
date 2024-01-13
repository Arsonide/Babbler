using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FMOD;

namespace Babbler.Implementation.Blurbs;

public static class BlurbSoundRegistry
{
    private static Dictionary<string, BlurbSound> Map = new Dictionary<string, BlurbSound>();

    public static void Initialize()
    {
        Map.Clear();
        string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException();
        
        // TODO fix directory structure with plugins and re-sort our blurb sounds. Also put them into a voice subdirectory.
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
        RESULT result = FMODUnity.RuntimeManager.CoreSystem.createSound(filePath, MODE.DEFAULT | MODE._3D, out Sound sound);

        // TODO we should check other usages of createX and see if we're checking this result consistently.
        if (result != RESULT.OK)
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