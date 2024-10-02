using Il2CppInterop.Runtime.Injection;
using BepInEx;
using BepInEx.Logging;
using SOD.Common.BepInEx;
using Babbler.Implementation.Phonetic;
using Babbler.Implementation.Common;
using Babbler.Implementation.Config;
using Babbler.Implementation.Emotes;
using Babbler.Implementation.Hosts;
using Babbler.Implementation.Occlusion.Vents;
using Babbler.Implementation.Synthesis;

namespace Babbler;

// There is an issue with Microsoft Speech Synthesis that causes it to crash the whole game if our plugin doesn't load first, before other plugins.
// Some unique interaction between BepInEx / Harmony and Microsoft Speech Synthesis that is deep in memory management.
// The only resolution I know of to fix this is appending AAAA here which causes us to load first, because Harmony patches in alphabetical order.
[BepInPlugin($"AAAA_{MyPluginInfo.PLUGIN_GUID}", MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class BabblerPlugin : PluginController<BabblerPlugin>
{
    private bool _hasInitializedImmediate;
    private bool _hasInitializedDeferred;
    
    public override void Load()
    {
        base.Load();

        _hasInitializedImmediate = false;
        _hasInitializedDeferred = false;
        
        BabblerConfig.Initialize(Config);
        
        if (!BabblerConfig.Enabled.Value)
        {
            Utilities.Log($"Plugin {MyPluginInfo.PLUGIN_GUID} is disabled.");
            return;
        }

        Utilities.Log($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        Harmony.PatchAll();
        Utilities.Log($"Plugin {MyPluginInfo.PLUGIN_GUID} is patched!");
        ClassInjector.RegisterTypeInIl2Cpp<SpeakerHost>();
        ClassInjector.RegisterTypeInIl2Cpp<VentTagPeekable>();
        ClassInjector.RegisterTypeInIl2Cpp<VentTagInteractable>();
        Utilities.Log($"Plugin {MyPluginInfo.PLUGIN_GUID} has added custom types!");
        
        InitializeImmediate();
    }

    public override bool Unload()
    {
        UninitializeImmediate();
        UninitializeDeferred();
        return base.Unload();
    }

    private void InitializeImmediate()
    {
        if (_hasInitializedImmediate)
        {
            return;
        }

        _hasInitializedImmediate = true;
        Utilities.Log("Plugin is running immediate initialization.", LogLevel.Debug);
        
        SpeakerHostPool.InitializePools();
        ReplacementRegistry.Initialize();
        
        // This must initialize the moment the game starts to "kickstart" Microsoft Speech Synthesis with a silent sound.
        // Without playing this sound immediately, the game will crash. I do not know why.
        if (BabblerConfig.Mode.Value == SpeechMode.Synthesis)
        {
            SynthesisVoiceRegistry.Initialize();
        }
    }

    public void InitializeDeferred()
    {
        if (_hasInitializedDeferred)
        {
            return;
        }

        _hasInitializedDeferred = true;
        Utilities.Log("Plugin is running deferred initialization.", LogLevel.Debug);

        // Wait for the main menu to load this stuff because FMOD's listener is ready at that time.
        FMODRegistry.Initialize();

        switch (BabblerConfig.Mode.Value)
        {
            case SpeechMode.Phonetic:
            case SpeechMode.Droning:
                PhoneticVoiceRegistry.Initialize();
                break;
        }
        
        EmoteSoundRegistry.Initialize();
        VentRegistry.Initialize();
        TVWatcher.Initialize();
    }

    private void UninitializeImmediate()
    {
        if (!_hasInitializedImmediate)
        {
            return;
        }
        
        Utilities.Log("Plugin is running immediate uninitialization.", LogLevel.Debug);
        SpeakerHostPool.UninitializePools();
    }

    private void UninitializeDeferred()
    {
        if (!_hasInitializedDeferred)
        {
            return;
        }
        
        Utilities.Log("Plugin is running deferred uninitialization.", LogLevel.Debug);

        switch (BabblerConfig.Mode.Value)
        {
            case SpeechMode.Phonetic:
            case SpeechMode.Droning:
                PhoneticVoiceRegistry.Uninitialize();
                break;
        }
        
        EmoteSoundRegistry.Uninitialize();
        VentRegistry.Uninitialize();
        TVWatcher.Uninitialize();
    }
}