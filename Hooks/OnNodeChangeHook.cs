using UnityEngine;
using HarmonyLib;
using Babbler.Implementation.Common;
using Babbler.Implementation.Config;
using Babbler.Implementation.Emotes;
using Babbler.Implementation.Hosts;
using Babbler.Implementation.Occlusion;
using BepInEx.Logging;

namespace Babbler.Hooks;

[HarmonyPatch(typeof(Human), "OnNodeChange")]
public class OnNodeChangeHook
{
    [HarmonyPostfix]
    public static void Postfix(Human __instance)
    {
        if (__instance.isPlayer)
        {
            OcclusionChecker.CachePlayerBounds();
        }

        if (!EmoteSoundRegistry.CanPlayIncidentals(__instance, true))
        {
            return;
        }
        
        if (__instance.drunk < BabblerConfig.IncidentalsMinDrunkForHiccups.Value)
        {
            return;
        }

        // Since this is based on distance travelled this makes them hiccup faster. Let's assume adrenaline is allowing them to surpass their drunkenness.
        if (__instance.isRunning || __instance.ai.inCombat)
        {
            return;
        }
        
        if (!EmoteSoundRegistry.IsEmoteRelevantBroadphase(__instance))
        {
            return;
        }
        
        float drunkAmount = Mathf.InverseLerp(BabblerConfig.IncidentalsMinDrunkForHiccups.Value, 1f, __instance.drunk);
        float threshold = Mathf.Lerp(BabblerConfig.IncidentalsMinHiccupChance.Value, BabblerConfig.IncidentalsMaxHiccupChance.Value, drunkAmount);
        
        // There are tons of drunk people in stairwells, this should lower the cacophony of hiccups a bit.
        if (__instance.currentGameLocation.isLobby && (__instance.currentNode.stairwellLowerLink || __instance.currentNode.stairwellUpperLink))
        {
            threshold /= 2;
        }
        
        if (Utilities.GlobalRandom.NextSingle() > threshold)
        {
            return;
        }

        SpeakerHostPool.Emotes.Play("hiccup", SoundContext.OverheardEmote, __instance);
    }
}