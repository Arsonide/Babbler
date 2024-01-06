using BepInEx;
using SOD.Common.BepInEx;

namespace Babbler;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class BabblerPlugin : PluginController<BabblerPlugin>
{
    public override void Load()
    {
        base.Load();
        
        Utilities.Log($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        
        Harmony.PatchAll();
        
        Utilities.Log($"Plugin {MyPluginInfo.PLUGIN_GUID} is patched!");
        
        // ClassInjector.RegisterTypeInIl2Cpp<ClassName>();
        // Utilities.Log($"Plugin {MyPluginInfo.PLUGIN_GUID} has added custom types!");
    }
}
