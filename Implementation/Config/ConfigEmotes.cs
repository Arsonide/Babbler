using BepInEx.Configuration;
using Babbler.Implementation.Common;

namespace Babbler.Implementation.Config;

public static partial class BabblerConfig
{
    public static ConfigEntry<bool> EmotesEnabled;
    public static ConfigEntry<string> EmotesTheme;
    
    public static ConfigEntry<float> EmotesMinStagger;
    public static ConfigEntry<float> EmotesMaxStagger;

    public static ConfigEntry<bool> EmotesUsePitchShifts;

    public static ConfigEntry<float> EmotesMinFrequencyMale;
    public static ConfigEntry<float> EmotesMaxFrequencyMale;
    
    public static ConfigEntry<float> EmotesMinFrequencyFemale;
    public static ConfigEntry<float> EmotesMaxFrequencyFemale;
    
    public static ConfigEntry<float> EmotesMinFrequencyNonBinary;
    public static ConfigEntry<float> EmotesMaxFrequencyNonBinary;

    private static void InitializeEmotes(ConfigFile config)
    {
        EmotesEnabled = config.Bind("7. Emotes", "Enabled", true,
                                     new ConfigDescription("Whether emote sound effects (like sneezing, sighing, or coughing) are enabled."));

        EmotesTheme = config.Bind("7. Emotes", "Theme", "Realistic",
                                    new ConfigDescription("Which theme to load and play for emote sound effects. Babbler comes with \"Realistic\" and \"Abstract\" but you can add more in the Emotes directory."));

        EmotesMinStagger = config.Bind("7. Emotes", "Min Stagger", 0.2f,
                                       new ConfigDescription("NPCs often emote at the same time, this is the minimum seconds to stagger their emote sounds a bit."));
        
        EmotesMaxStagger = config.Bind("7. Emotes", "Max Stagger", 0.6f,
                                       new ConfigDescription("NPCs often emote at the same time, this is the maximum seconds to stagger their emote sounds a bit."));

        EmotesUsePitchShifts = config.Bind("7. Emotes", "Use Pitch Shifts", false,
                                           new ConfigDescription("Whether or not to shift the pitch of emote sound effects to try and match an NPC's voice."));
        
        EmotesMinFrequencyMale = config.Bind("7. Emotes", "Min Frequency Male", 100f,
                                              new ConfigDescription("Lowest possible frequency (in hertz) for male emote sound effects."));
        
        EmotesMaxFrequencyMale = config.Bind("7. Emotes", "Max Frequency Male", 180f,
                                              new ConfigDescription("Highest possible frequency (in hertz) for male emote sound effects."));
        
        EmotesMinFrequencyFemale = config.Bind("7. Emotes", "Min Frequency Female", 165f,
                                                new ConfigDescription("Lowest possible frequency (in hertz) for female emote sound effects."));
        
        EmotesMaxFrequencyFemale = config.Bind("7. Emotes", "Max Frequency Female", 255f,
                                                new ConfigDescription("Highest possible frequency (in hertz) for female emote sound effects."));
        
        EmotesMinFrequencyNonBinary = config.Bind("7. Emotes", "Min Frequency Non-Binary", 100f,
                                                   new ConfigDescription("Lowest possible frequency (in hertz) for non-binary emote sound effects."));
        
        EmotesMaxFrequencyNonBinary = config.Bind("7. Emotes", "Max Frequency Non-Binary", 255f,
                                                   new ConfigDescription("Highest possible frequency (in hertz) for non-binary emote sound effects."));
        
        Utilities.EnforceMinMax(ref EmotesMinStagger, ref EmotesMaxStagger);
        Utilities.EnforceMinMax(ref EmotesMinFrequencyMale, ref EmotesMaxFrequencyMale);
        Utilities.EnforceMinMax(ref EmotesMinFrequencyFemale, ref EmotesMaxFrequencyFemale);
        Utilities.EnforceMinMax(ref EmotesMinFrequencyNonBinary, ref EmotesMaxFrequencyNonBinary);
    }

    private static void ResetEmotes()
    {
        EmotesEnabled.Value = (bool)EmotesEnabled.DefaultValue;
        EmotesTheme.Value = (string)EmotesTheme.DefaultValue;
        EmotesMinStagger.Value = (float)EmotesMinStagger.DefaultValue;
        EmotesMaxStagger.Value = (float)EmotesMaxStagger.DefaultValue;
        EmotesUsePitchShifts.Value = (bool)EmotesUsePitchShifts.DefaultValue;
        EmotesMinFrequencyMale.Value = (float)EmotesMinFrequencyMale.DefaultValue;
        EmotesMaxFrequencyMale.Value = (float)EmotesMaxFrequencyMale.DefaultValue;
        EmotesMinFrequencyFemale.Value = (float)EmotesMinFrequencyFemale.DefaultValue;
        EmotesMaxFrequencyFemale.Value = (float)EmotesMaxFrequencyFemale.DefaultValue;
        EmotesMinFrequencyNonBinary.Value = (float)EmotesMinFrequencyNonBinary.DefaultValue;
        EmotesMaxFrequencyNonBinary.Value = (float)EmotesMaxFrequencyNonBinary.DefaultValue;
    }
}