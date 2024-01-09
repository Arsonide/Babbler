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

        Utilities.Log($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        
        Harmony.PatchAll();
        
        Utilities.Log($"Plugin {MyPluginInfo.PLUGIN_GUID} is patched!");
        
        ClassInjector.RegisterTypeInIl2Cpp<Babbler>();
        Utilities.Log($"Plugin {MyPluginInfo.PLUGIN_GUID} has added custom types!");
        
        FMODReferences.Initialize();
        PhoneticSoundDatabase.Initialize();
    }

    public override bool Unload()
    {
        PhoneticSoundDatabase.Uninitialize();
        return base.Unload();
    }
}