using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using BepInEx.Logging;
using Babbler.Implementation.Characteristics;
using Babbler.Implementation.Common;
using Babbler.Implementation.Config;

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

    private static bool IsPlayerNear(Human human, float nearRange)
    {
        if (!human.currentCityTile.isInPlayerVicinity)
        {
            return false;
        }
        
        return Vector3.Distance(human.aimTransform.position, Player.Instance.aimTransform.position) < nearRange;
    }
    
    public static bool IsEmoteRelevantBroadphase(Human human, float nearRange)
    {
        if (!BabblerConfig.EmotesEnabled.Value)
        {
            return false;
        }

        if (human.currentCityTile == null || human.isPlayer || human.isDead)
        {
            return false;
        }
        
        if (human.visible)
        {
            return IsPlayerNear(human, nearRange);
        }

        if (human.isOnStreet && Player.Instance.isOnStreet)
        {
            return IsPlayerNear(human, nearRange);
        }

        if (human.currentCityTile.cityCoord != Player.Instance.currentCityTile.cityCoord)
        {
            return false;
        }

        NewAddress humanAddress = human?.currentGameLocation?.thisAsAddress;
        NewAddress playerAddress = Player.Instance?.currentGameLocation?.thisAsAddress;

        if (humanAddress == null || playerAddress == null)
        {
            return false;
        }
        
        if (humanAddress.isLobby && playerAddress.isLobby)
        {
            return IsPlayerNear(human, nearRange);
        }

        if (humanAddress.id == playerAddress.id)
        {
            return IsPlayerNear(human, nearRange);
        }

        return false;
    }
    
    public static bool ShouldPlayUncouthEmote(Human human, float minThreshold, float maxThreshold)
    {
        float conscientiousness = human.conscientiousness;
        float drunkenness = human.drunk;

        if (human.isHome)
        {
            conscientiousness /= 2f;
        }
        
        // We want whatever is lower, your conscientiousness or the inverse of your drunkenness, and then we want to invert that.
        float uncouthness = 1f - Mathf.Min(conscientiousness, 1f - drunkenness);
        float threshold = Mathf.Lerp(minThreshold, maxThreshold, uncouthness);
        
        return Utilities.GlobalRandom.NextSingle() <= threshold;
    }
}