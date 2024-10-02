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
        switch (newState)
        {
            case CitizenAnimationController.IdleAnimationState.sitting:
                if (ShouldPlayBathroomIncidental(__instance.cit, newState))
                {
                    SpeakerHostPool.Emotes.Play("fart", SoundContext.OverheardEmote, __instance.cit);
                }
                
                break;
            case CitizenAnimationController.IdleAnimationState.showering:
                if (ShouldPlayBathroomIncidental(__instance.cit, newState))
                {
                    SpeakerHostPool.Emotes.Play("whistling", SoundContext.OverheardEmote, __instance.cit);
                }
                
                break;
        }
    }

    private static bool ShouldPlayBathroomIncidental(Human human, CitizenAnimationController.IdleAnimationState state)
    {
        if (!BabblerConfig.IncidentalsEnabled.Value)
        {
            return false;
        }

        if (!EmoteSoundRegistry.IsEmoteRelevantBroadphase(human))
        {
            return false;
        }

        switch (state)
        {
            case CitizenAnimationController.IdleAnimationState.sitting:
                if (!EmoteSoundRegistry.ShouldPlayUncouthEmote(human, BabblerConfig.IncidentalsMinFartChance.Value, BabblerConfig.IncidentalsMaxFartChance.Value))
                {
                    return false;
                }

                break;
            case CitizenAnimationController.IdleAnimationState.showering:
                if (!EmoteSoundRegistry.ShouldPlayExpressiveEmote(human, BabblerConfig.IncidentalsMinWhistleChance.Value, BabblerConfig.IncidentalsMaxWhistleChance.Value))
                {
                    return false;
                }

                break;
        }

        string roomName = human.currentRoom.roomType.presetName.ToLowerInvariant();

        if (!roomName.Contains("bathroom") && !roomName.Contains("shower"))
        {
            return false;
        }

        return true;
    }
}