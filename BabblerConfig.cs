using BepInEx.Configuration;

namespace Babbler;

public static class BabblerConfig
{
    public static bool Enabled = true;
    public static float FirstPartyVolume = 0.7f;
    public static float ThirdPartyVolume = 0.3f;
    
    public static void Initialize(ConfigFile config)
    {
        Enabled = config.Bind("General", "Enabled", true).Value;
        FirstPartyVolume = config.Bind("Audio", "First Party Babble Volume", 0.7f).Value;
        ThirdPartyVolume = config.Bind("Audio", "Third Party Babble Volume", 0.3f).Value;
    }
}