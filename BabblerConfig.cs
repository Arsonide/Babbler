using BepInEx.Configuration;

namespace Babbler;

public static class BabblerConfig
{
    public static bool Enabled = true;
    public static float ConversationalVolume = 0.7f;
    public static float PhoneVolume = 0.5f;
    public static float OverheardVolume = 0.3f;
    public static float SyllableSpeed = 0.2f;
    public static float MinimumPitch = 0.65f;
    public static float MaximumPitch = 3f;
    
    public static void Initialize(ConfigFile config)
    {
        Enabled = config.Bind("General", "Enabled", true).Value;
        ConversationalVolume = config.Bind("Audio", "Conversational Volume", 0.7f).Value;
        PhoneVolume = config.Bind("Audio", "Phone Volume", 0.5f).Value;
        OverheardVolume = config.Bind("Audio", "Overheard Volume", 0.3f).Value;
        SyllableSpeed = config.Bind("Audio", "Syllable Speed", 0.2f).Value;
        MinimumPitch = config.Bind("Audio", "Minimum Pitch", 0.65f).Value;
        MaximumPitch = config.Bind("Audio", "Maximum Pitch", 3f).Value;
    }
}