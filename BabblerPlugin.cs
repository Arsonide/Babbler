using BepInEx;
using Il2CppInterop.Runtime.Injection;
using SOD.Common.BepInEx;

namespace Babbler;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class BabblerPlugin : PluginController<BabblerPlugin>
{
    public override void Load()
    {
        base.Load();

        BabblerConfig.Initialize(Config);
        
        if (!BabblerConfig.Enabled)
        {
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is disabled.");
            return;
        }

        Harmony.PatchAll();
        ClassInjector.RegisterTypeInIl2Cpp<SpeakerHost>();
        
        Utilities.Log($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    public override bool Unload()
    {
        PhoneticSoundDatabase.Uninitialize();
        return base.Unload();
    }
}