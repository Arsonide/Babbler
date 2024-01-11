using HarmonyLib;

namespace Babbler;

[HarmonyPatch(typeof(MainMenuController), "Start")]
public class MainMenuControllerPatch
{
    private static bool initialized;

    public static void Postfix()
    {
        if (initialized)
        {
            return;
        }

        initialized = true;

        // Wait for the main menu to load this stuff because FMOD is ready at that time.
        FMODReferences.Initialize();
        PhoneticSoundDatabase.Initialize();
    }
}