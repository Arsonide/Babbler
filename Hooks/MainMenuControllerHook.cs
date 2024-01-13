using HarmonyLib;

namespace Babbler.Hooks;

[HarmonyPatch(typeof(MainMenuController), "Start")]
public class MainMenuControllerHook
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        BabblerPlugin.Instance.InitializeDeferred();
    }
}