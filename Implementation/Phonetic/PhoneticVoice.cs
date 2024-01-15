using System;
using System.Collections.Generic;
using System.IO;
using FMOD;
using BepInEx.Logging;
using Babbler.Implementation.Common;

namespace Babbler.Implementation.Phonetic;

public class PhoneticVoice
{
    public string Name { get; private set; }
    public float Frequency { get; private set; }
    
    private readonly Dictionary<string, PhoneticSound> _phonemes = new Dictionary<string, PhoneticSound>();

    public void Initialize(string directory)
    {
        _phonemes.Clear();

        if (!Directory.Exists(directory))
        {
            return;
        }
        
        string directoryName = Path.GetFileNameWithoutExtension(directory);
        string[] directorySegments = directoryName.Split('_', StringSplitOptions.RemoveEmptyEntries);

        if (directorySegments.Length != 2)
        {
            return;
        }

        if (!float.TryParse(directorySegments[1], out float frequency))
        {
            return;
        }

        Frequency = frequency;
        Name = directorySegments[0];
        
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
            if (noExtension.StartsWith("symbol"))
            {
                if (phonetic.Contains("space"))
                {
                    _phonemes[" "] = newPhonetic;
                    _phonemes[","] = newPhonetic;
                }
                else if (phonetic.Contains("exclamation"))
                {
                    _phonemes["!"] = newPhonetic;
                }
                else if (phonetic.Contains("question"))
                {
                    _phonemes["?"] = newPhonetic;
                }
                else if (phonetic.Contains("period"))
                {
                    _phonemes["."] = newPhonetic;
                }
            }
            else
            {
                _phonemes[phonetic] = newPhonetic;
            }
        }
    }

    public void Uninitialize()
    {
        foreach (KeyValuePair<string, PhoneticSound> pair in _phonemes)
        {
            if (pair.Value.Released)
            {
                continue;
            }

            pair.Value.Sound.release();
            pair.Value.Released = true;
        }
    }
    
    private PhoneticSound CreatePhoneticSound(string filePath, string phonetic)
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

    public bool TryGetPhoneticSound(string phonetic, out PhoneticSound result)
    {
        return _phonemes.TryGetValue(phonetic, out result);
    }
}