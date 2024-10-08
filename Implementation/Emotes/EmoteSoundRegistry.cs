﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using BepInEx.Logging;
using Babbler.Implementation.Characteristics;
using Babbler.Implementation.Common;
using Babbler.Implementation.Config;
using Babbler.Implementation.Occlusion;

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
    
    public static bool TryGetEmote(string key, Human human, out VoiceCharacteristics characteristics, out EmoteSound sound)
    {
        if (!Groups.TryGetValue(key, out EmoteSoundFamily group))
        {
            characteristics = default;
            sound = null;
            return false;
        }
        
        characteristics = VoiceCharacteristics.Create(human, group.HasMaleEmotes, group.HasFemaleEmotes, group.HasNonBinaryEmotes);
        sound = group.GetRandomSound(characteristics);
        return true;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmoteRelevantBroadphase(Human human)
    {
        if (!BabblerConfig.EmotesEnabled.Value)
        {
            return false;
        }

        // This method is only used from hooks for incidentals, that would mean the player was not on the phone or speaking directly with them.
        OcclusionResult occlusion = OcclusionChecker.CheckOcclusion(human, Player.Instance, SoundContext.OverheardEmote);
        return occlusion.State != OcclusionState.FullOcclusion;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CanPlayIncidentals(Human human, bool mouthVocalization)
    {
        if (!BabblerConfig.IncidentalsEnabled.Value)
        {
            return false;
        }

        if (mouthVocalization && human.isSpeaking)
        {
            return false;
        }
        
        return !human.isPlayer && !human.isAsleep && !human.isDead && !human.isStunned;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ShouldPlayExpressiveEmote(Human human, float minThreshold, float maxThreshold)
    {
        float expressiveness = (human.extraversion + human.creativity) / 2f;
        float threshold = Mathf.Lerp(minThreshold, maxThreshold, expressiveness);
        
        return Utilities.GlobalRandom.NextSingle() <= threshold;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ShouldPlayExtravertedEmote(Human human, float minThreshold, float maxThreshold)
    {
        float threshold = Mathf.Lerp(minThreshold, maxThreshold, human.extraversion);
        
        return Utilities.GlobalRandom.NextSingle() <= threshold;
    }
}