using Babbler.Implementation.Common;
using HarmonyLib;
using UnityEngine;

namespace Babbler.Hooks;

[HarmonyPatch(typeof(PlayerPrefsController), "OnToggleChanged")]
public class VolumeCacheHook
{
    public static float MasterVolume => _masterVolume;
    public static float OtherVolume => _otherVolume;
    
    private static float _masterVolume;
    private static float _otherVolume;

    private const string MASTER_KEY = "masterVolume";
    private const string OTHER_KEY = "otherVolume";
    
    [HarmonyPostfix]
    public static void Postfix(PlayerPrefsController __instance, string id, bool fetchValueFromControls, MonoBehaviour elementScript)
    {
        switch (id)
        {
            case MASTER_KEY:
                _masterVolume = CacheVolume(MASTER_KEY);
                break;
            case OTHER_KEY:
                _otherVolume = CacheVolume(OTHER_KEY);
                break;
        }
    }

    private static float CacheVolume(string key)
    {
        int value = PlayerPrefs.GetInt(key, 100);
        return (float)value / 100f;
    }

    public static void Initialize()
    {
        _masterVolume = CacheVolume(MASTER_KEY);
        _otherVolume = CacheVolume(OTHER_KEY);
    }
}