using UnityEngine;
using HarmonyLib;
using Babbler.Implementation.Common;
using Babbler.Implementation.Config;
using Babbler.Implementation.Emotes;
using Babbler.Implementation.Hosts;
using Babbler.Implementation.Occlusion;

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
        
        if (!BabblerConfig.IncidentalsEnabled.Value)
        {
            return;
        }
        
        if (__instance.drunk < BabblerConfig.IncidentalsMinDrunkForHiccups.Value)
        {
            return;
        }
        
        if (!EmoteSoundRegistry.IsEmoteRelevantBroadphase(__instance))
        {
            return;
        }
        
        float drunkAmount = Mathf.InverseLerp(BabblerConfig.IncidentalsMinDrunkForHiccups.Value, 1f, __instance.drunk);
        float threshold = Mathf.Lerp(BabblerConfig.IncidentalsMinHiccupChance.Value, BabblerConfig.IncidentalsMaxHiccupChance.Value, drunkAmount);
        
        if (Utilities.GlobalRandom.NextSingle() > threshold)
        {
            return;
        }

        SpeakerHostPool.Emotes.Play("hiccup", SoundContext.OverheardEmote, __instance);
    }
}