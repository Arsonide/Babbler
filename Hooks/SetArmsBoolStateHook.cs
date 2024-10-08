﻿using UnityEngine;
using HarmonyLib;
using Babbler.Implementation.Common;
using Babbler.Implementation.Config;
using Babbler.Implementation.Emotes;
using Babbler.Implementation.Hosts;

namespace Babbler.Hooks;

[HarmonyPatch(typeof(CitizenAnimationController), "SetArmsBoolState")]
public class SetArmsBoolStateHook
{
    [HarmonyPrefix]
    public static bool Prefix(CitizenAnimationController __instance, CitizenAnimationController.ArmsBoolSate newState)
    {
        CitizenAnimationController.ArmsBoolSate oldState = __instance.armsBoolAnimationState;

        if (oldState != CitizenAnimationController.ArmsBoolSate.armsConsuming || newState == CitizenAnimationController.ArmsBoolSate.armsConsuming)
        {
            return true;
        }

        if (!EmoteSoundRegistry.CanPlayIncidentals(__instance.cit, true))
        {
            return true;
        }
        
        if (!EmoteSoundRegistry.IsEmoteRelevantBroadphase(__instance.cit))
        {
            return true;
        }
        
        if (!EmoteSoundRegistry.ShouldPlayUncouthEmote(__instance.cit, BabblerConfig.IncidentalsMinBurpChance.Value, BabblerConfig.IncidentalsMaxBurpChance.Value))
        {
            return true;
        }

        SpeakerHostPool.Emotes.Play("burp", SoundContext.OverheardEmote, __instance.cit, Utilities.GlobalRandom.Next(0, 6));
        return true;
    }
}