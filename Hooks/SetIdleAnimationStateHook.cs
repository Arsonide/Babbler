using UnityEngine;
using HarmonyLib;
using Babbler.Implementation.Common;
using Babbler.Implementation.Config;
using Babbler.Implementation.Emotes;
using Babbler.Implementation.Hosts;

namespace Babbler.Hooks;

[HarmonyPatch(typeof(CitizenAnimationController), "SetIdleAnimationState")]
public class SetIdleAnimationStateHook
{
    [HarmonyPostfix]
    public static void Postfix(CitizenAnimationController __instance, CitizenAnimationController.IdleAnimationState newState)
    {
        if (newState != CitizenAnimationController.IdleAnimationState.sitting)
        {
            return;
        }

        if (!__instance.cit.currentCityTile.isInPlayerVicinity)
        {
            return;
        }
        
        if (!BabblerConfig.IncidentalsEnabled.Value)
        {
            return;
        }

        if (!EmoteSoundRegistry.ShouldPlayUncouthEmote(__instance.cit, BabblerConfig.IncidentalsMinFartChance.Value, BabblerConfig.IncidentalsMaxFartChance.Value))
        {
            return;
        }
        
        if (Vector3.Distance(__instance.cit.aimTransform.position, Player.Instance.aimTransform.position) > BabblerConfig.IncidentalsRange.Value)
        {
            return;
        }

        string roomName = __instance.cit.currentRoom.roomType.presetName.ToLowerInvariant();

        if (!roomName.Contains("bathroom") && !roomName.Contains("shower"))
        {
            return;
        }
        
        SpeakerHostPool.Emotes.Play("fart", SoundContext.OverheardEmote, __instance.cit);
    }
}