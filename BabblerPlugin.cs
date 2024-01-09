using BepInEx;
using Il2CppInterop.Runtime.Injection;
using SOD.Common.BepInEx;

namespace Babbler;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class BabblerPlugin : PluginController<BabblerPlugin>
{
    public static float FirstPartyVolume = 0.7f;
    public static float ThirdPartyVolume = 0.3f;
    
    public override void Load()
    {
        base.Load();

        if (!Config.Bind("General", "Enabled", true).Value)
        {
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is disabled.");
            return;
        }

        FirstPartyVolume = Config.Bind("Audio", "First Party Babble Volume", 0.7f).Value;
        ThirdPartyVolume = Config.Bind("Audio", "Third Party Babble Volume", 0.3f).Value;

        Utilities.Log($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        
        Harmony.PatchAll();
        
        Utilities.Log($"Plugin {MyPluginInfo.PLUGIN_GUID} is patched!");
        
        ClassInjector.RegisterTypeInIl2Cpp<Babbler>();
        Utilities.Log($"Plugin {MyPluginInfo.PLUGIN_GUID} has added custom types!");
        
        FMODReferences.Initialize();
        PhoneticSoundDatabase.LoadPhonetics();
    }

    public override bool Unload()
    {
        PhoneticSoundDatabase.UnloadPhonetics();
        return base.Unload();
    }
}