using BepInEx.Configuration;

namespace Babbler;

public static class BabblerConfig
{
    public static bool Enabled = true;
    
    public static float SyllableSpeed = 0.2f;
    public static bool DistortPhoneSpeech = true;
    
    public static float ConversationalVolume = 0.7f;
    public static float PhoneVolume = 0.5f;
    public static float OverheardVolume = 0.3f;

    public static float MinimumPitch = 0.65f;
    public static float MaximumPitch = 3f;

    public static void Initialize(ConfigFile config)
    {
        Enabled = config.Bind("General", "Enabled", true).Value;
        
        SyllableSpeed = config.Bind("Speech", "Syllable Speed", 0.2f).Value;
        DistortPhoneSpeech = config.Bind("Speech", "Distort Phone Speech", true).Value;

        ConversationalVolume = config.Bind("Volume", "Conversational Volume", 0.7f).Value;
        PhoneVolume = config.Bind("Volume", "Phone Volume", 0.5f).Value;
        OverheardVolume = config.Bind("Volume", "Overheard Volume", 0.3f).Value;
        
        MinimumPitch = config.Bind("Pitch", "Minimum Pitch", 0.65f).Value;
        MaximumPitch = config.Bind("Pitch", "Maximum Pitch", 3f).Value;
    }
}