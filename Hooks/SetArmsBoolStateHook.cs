using UnityEngine;
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

        if (!__instance.cit.currentCityTile.isInPlayerVicinity)
        {
            return true;
        }
        
        if (!BabblerConfig.IncidentalsEnabled.Value)
        {
            return true;
        }
        
        if (!EmoteSoundRegistry.ShouldPlayUncouthEmote(__instance.cit, BabblerConfig.IncidentalsMinBurpChance.Value, BabblerConfig.IncidentalsMaxBurpChance.Value))
        {
            return true;
        }

        if (Vector3.Distance(__instance.cit.aimTransform.position, Player.Instance.aimTransform.position) > BabblerConfig.IncidentalsRange.Value)
        {
            return true;
        }
        
        SpeakerHostPool.Emotes.Play("burp", SoundContext.OverheardEmote, __instance.cit, Utilities.GlobalRandom.Next(0, 6));
        return true;
    }
}