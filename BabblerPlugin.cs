using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using FMOD;
using Il2CppInterop.Runtime.Injection;
using SOD.Common.BepInEx;

namespace Babbler;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class BabblerPlugin : PluginController<BabblerPlugin>
{
    private static Dictionary<string, BabblePhonetic> PhoneticMap = new Dictionary<string, BabblePhonetic>();
    private static ChannelGroup MasterGroup;

    public override void Load()
    {
        base.Load();
        
        Utilities.Log($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        
        Harmony.PatchAll();
        
        Utilities.Log($"Plugin {MyPluginInfo.PLUGIN_GUID} is patched!");
        
        ClassInjector.RegisterTypeInIl2Cpp<Babbler>();
        Utilities.Log($"Plugin {MyPluginInfo.PLUGIN_GUID} has added custom types!");
        
        LoadPhonetics();
        FMODUnity.RuntimeManager.CoreSystem.getMasterChannelGroup(out MasterGroup);
    }

    public override bool Unload()
    {
        UnloadPhonetics();
        return base.Unload();
    }

    private static void LoadPhonetics()
    {
        PhoneticMap.Clear();
        string directory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException(), "sounds");
        
        foreach (string filePath in Directory.GetFiles(directory, "*.wav"))
        {
            string noExtension = Path.GetFileNameWithoutExtension(filePath);
            string[] split = noExtension.Split('_');

            if (split.Length != 2)
            {
                continue;
            }

            string phonetic = split[1].ToLowerInvariant();
            BabblePhonetic newPhonetic = CreatePhonetic(filePath, phonetic);
            
            // Space is used for all punctuation marks. Otherwise, the phonetic is the phonetic.
            if (phonetic.Contains("space"))
            {
                PhoneticMap[" "] = newPhonetic;
                PhoneticMap[","] = newPhonetic;
                PhoneticMap["."] = newPhonetic;
                PhoneticMap["?"] = newPhonetic;
                PhoneticMap["!"] = newPhonetic;
            }
            else
            {
                PhoneticMap[phonetic] = newPhonetic;
            }
        }
    }

    private static void UnloadPhonetics()
    {
        foreach (KeyValuePair<string, BabblePhonetic> pair in PhoneticMap)
        {
            if (!pair.Value.Released)
            {
                pair.Value.Sound.release();
                pair.Value.Released = true;
            }
        }
    }

    private static BabblePhonetic CreatePhonetic(string filePath, string phonetic)
    {
        RESULT result = FMODUnity.RuntimeManager.CoreSystem.createSound(filePath, MODE.DEFAULT | MODE._3D, out Sound sound);

        if (result != RESULT.OK)
        {
            return null;
        }
        
        sound.getLength(out uint length, TIMEUNIT.MS);
        
        BabblePhonetic newPhonetic = new BabblePhonetic()
        {
            Phonetic = phonetic, FilePath = filePath, Sound = sound, Length = length / 1000f, Released = false,
        };

        return newPhonetic;
    }

    public static bool TryGetPhonetic(string phonetic, out BabblePhonetic result)
    {
        return PhoneticMap.TryGetValue(phonetic, out result);
    }

    public static ChannelGroup GetChannelGroup()
    {
        return MasterGroup;
    }
}