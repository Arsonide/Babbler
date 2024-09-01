using System.Collections.Generic;
using System.IO;
using Babbler.Implementation.Characteristics;
using FMOD;
using Babbler.Implementation.Common;

namespace Babbler.Implementation.Emotes;

public class EmoteSoundFamily
{
    public string Key { get; private set; }
    
    private readonly List<EmoteSound> _allSounds = new List<EmoteSound>();
    private readonly List<EmoteSound> _maleSounds = new List<EmoteSound>();
    private readonly List<EmoteSound> _femaleSounds = new List<EmoteSound>();
    private readonly List<EmoteSound> _nonBinarySounds = new List<EmoteSound>();

    private static readonly List<EmoteSound>[] EmotePriorities = new List<EmoteSound>[4];

    public bool HasMaleEmotes => _maleSounds.Count > 0;
    public bool HasFemaleEmotes => _femaleSounds.Count > 0;
    public bool HasNonBinaryEmotes => _nonBinarySounds.Count > 0;
    
    public void Initialize(string directory)
    {
        _allSounds.Clear();
        _maleSounds.Clear();
        _femaleSounds.Clear();
        _nonBinarySounds.Clear();
        
        if (!Directory.Exists(directory))
        {
            return;
        }
        
        Key = Path.GetFileNameWithoutExtension(directory).ToLowerInvariant();

        foreach (string filePath in Directory.GetFiles(directory, "*.wav"))
        {
            EmoteSound newEmote = CreateEmoteSound(filePath, Key);
            
            if (newEmote == null)
            {
                continue;
            }

            switch (newEmote.Category)
            {
                case VoiceCategory.Male:
                    _maleSounds.Add(newEmote);
                    break;
                case VoiceCategory.Female:
                    _femaleSounds.Add(newEmote);
                    break;
                default:
                    _nonBinarySounds.Add(newEmote);
                    break;
            }
            
            _allSounds.Add(newEmote);
        }
    }

    public void Uninitialize()
    {
        foreach (EmoteSound sound in _allSounds)
        {
            if (sound.Released)
            {
                continue;
            }

            sound.Sound.release();
            sound.Released = true;
        }
    }

    private EmoteSound CreateEmoteSound(string filePath, string key)
    {
        if (!FMODRegistry.TryCreateSound(filePath, MODE.DEFAULT | MODE._3D, out Sound sound))
        {
            return null;
        }
        
        string noExtension = Path.GetFileNameWithoutExtension(filePath);
        string[] split = noExtension.Split('_');

        if (split.Length != 3)
        {
            return null;
        }

        string categoryString = split[1].ToLowerInvariant();
        string frequencyString = split[2].ToLowerInvariant();
        
        VoiceCategory category = VoiceCategory.NonBinary;
        bool canPitchShift = true;
        
        switch (categoryString)
        {
            case "male":
                category = VoiceCategory.Male;
                break;
            case "female":
                category = VoiceCategory.Female;
                break;
        }

        if (!float.TryParse(frequencyString, out float frequency))
        {
            frequency = 178f;
            canPitchShift = false;
        }
        
        sound.getLength(out uint length, TIMEUNIT.MS);
        float floatLength = length / 1000f;
        
        EmoteSound newEmote = new EmoteSound()
        {
            Key = key,
            Category = category,
            Frequency = frequency,
            CanPitchShift = canPitchShift,
            FilePath = filePath,
            Sound = sound,
            Length = floatLength,
            Released = false,
        };

        return newEmote;
    }

    public EmoteSound GetRandomSound(VoiceCharacteristics characteristics)
    {
        return characteristics.SelectRandomGenderedListElement(EmotePriorities, _allSounds, _maleSounds, _femaleSounds, _nonBinarySounds);
    }
}