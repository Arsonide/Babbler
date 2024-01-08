using HarmonyLib;

namespace Babbler.Patches;

[HarmonyPatch(typeof(SpeechBubbleController), "Setup")]
public class SpeechBubbleControllerPatch
{
    public static void Postfix(SpeechBubbleController __instance)
    {
        string babbleInput = __instance?.actualString;

        if (string.IsNullOrWhiteSpace(babbleInput))
        {
            return;
        }

        // This is used for things like [Sneeze] or [Sigh]. Don't babble for emotes.
        if (babbleInput.StartsWith("[") && babbleInput.EndsWith("]"))
        {
            return;
        }
        
        // Search around for a human because they don't seem to be assigned consistently?
        Human human = __instance?.speechController?.actor as Human ?? __instance?.speechController?.actor?.ai?.human;
        BabblerPool.Play(human, __instance.actualString);
    }
}