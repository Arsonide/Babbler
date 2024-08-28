using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Babbler.Implementation.Characteristics;
using Babbler.Implementation.Common;
using Babbler.Implementation.Config;
using BepInEx.Logging;

namespace Babbler.Implementation.Emotes;

public static class EmoteSoundRegistry
{
    private static readonly Dictionary<string, EmoteSoundFamily> Groups = new Dictionary<string, EmoteSoundFamily>();
    
    public static void Initialize()
    {
        Groups.Clear();

        if (BabblerConfig.EmotesEnabled.Value)
        {
            string directory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException(), "Emotes", BabblerConfig.EmotesTheme.Value);

            if (Directory.Exists(directory))
            {
                foreach (string subdirectory in Directory.GetDirectories(directory))
                {
                    EmoteSoundFamily family = new EmoteSoundFamily();
                    family.Initialize(subdirectory);
                    Groups.Add(family.Key, family);
                }
            }
        }

        Utilities.Log($"EmoteSoundRegistry has initialized! Sounds: {Groups.Count}", LogLevel.Debug);
    }

    public static void Uninitialize()
    {
        foreach (EmoteSoundFamily group in Groups.Values)
        {
            group.Uninitialize();
        }
    }

    public static bool HasEmote(string key)
    {
        return Groups.ContainsKey(key);
    }
    
    public static EmoteSound GetEmote(string key, Human human, out VoiceCharacteristics characteristics)
    {
        if (!Groups.TryGetValue(key, out EmoteSoundFamily group))
        {
            characteristics = default;
            return null;
        }
        
        characteristics = VoiceCharacteristics.Create(human, group.HasMaleEmotes, group.HasFemaleEmotes, group.HasNonBinaryEmotes);
        return group.GetRandomSound(characteristics);
    }
}