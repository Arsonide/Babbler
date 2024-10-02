using HarmonyLib;
using Babbler.Implementation.Occlusion.Vents;

namespace Babbler.Hooks;

[HarmonyPatch(typeof(AudioController), "UpdateMixing")]
public class UpdateMixingHook
{
    [HarmonyPostfix]
    public static void Postfix(AudioController __instance)
    {
        VentRegistry.Tick();
    }
}