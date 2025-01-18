using BepInEx.Configuration;
using Babbler.Implementation.Common;
using Babbler.Implementation.Synthesis;

namespace Babbler.Implementation.Config;

public static partial class BabblerConfig
{
    public static ConfigEntry<SynthesisVoiceFilterType> SynthesisVoiceFilter;
    public static ConfigEntry<string> SynthesisVoiceFilterInput;

    public static ConfigEntry<string> SynthesisVoiceLocaleFilter;
    public static ConfigEntry<bool> SynthesisDebugVoiceName;

    public static ConfigEntry<int> SynthesisMinSpeed;
    public static ConfigEntry<int> SynthesisMaxSpeed;
    
    public static ConfigEntry<float> SynthesisMinPitchMale;
    public static ConfigEntry<float> SynthesisMaxPitchMale;

    public static ConfigEntry<float> SynthesisMinPitchFemale;
    public static ConfigEntry<float> SynthesisMaxPitchFemale;

    public static ConfigEntry<float> SynthesisMinPitchNonBinary;
    public static ConfigEntry<float> SynthesisMaxPitchNonBinary;
    
    private static void InitializeSynthesis(ConfigFile config)
    {
        SynthesisVoiceFilter = config.Bind("4. Synthesis", "Voice Filter Type", SynthesisVoiceFilterType.Everything,
                                           new ConfigDescription("Determines which installed voices on Windows Babbler will use. Set to \"Everything\" for all installed voices, \"Blacklist\" to block some, or \"Whitelist\" to only allow some."));

        SynthesisVoiceFilterInput = config.Bind("4. Synthesis", "Voice Filter", string.Empty,
                                                new ConfigDescription("If filter type is set to blacklist or whitelist, this is where you put the names you want to filter for in, separated by semicolons. The names are case-insensitive and flexible, so \"david\" matches \"Microsoft David\", etc."));

        SynthesisVoiceLocaleFilter = config.Bind("4. Synthesis", "Voice Language Filter", "en",
                                                  new ConfigDescription("A semicolon-separated list of voice locales to use."));

        SynthesisDebugVoiceName = config.Bind("4. Synthesis", "Debug Voice Name", false,
                                              new ConfigDescription("If true, all speakers will say what voice they're using when they speak."));

        SynthesisMinSpeed = config.Bind("4. Synthesis", "Min Speed", -2,
                                        new ConfigDescription("Lowest possible speed for speech. Zero being the standard speed.",
                                                              new AcceptableValueRange<int>(-10, 10)));
        
        SynthesisMaxSpeed = config.Bind("4. Synthesis", "Max Speed", 3,
                                        new ConfigDescription("Highest possible speed for speech. Zero being the standard speed.", 
                                                              new AcceptableValueRange<int>(-10, 10)));
        
        SynthesisMinPitchMale = config.Bind("4. Synthesis", "Min Pitch Male", 0.75f,
                                                new ConfigDescription("Lowest possible pitch (relative percent) for male voices."));
        
        SynthesisMaxPitchMale = config.Bind("4. Synthesis", "Max Pitch Male", 1.25f,
                                                new ConfigDescription("Highest possible pitch (relative percent) for male voices."));
        
        SynthesisMinPitchFemale = config.Bind("4. Synthesis", "Min Pitch Female", 0.75f,
                                                  new ConfigDescription("Lowest possible pitch (relative percent) for female voices."));
        
        SynthesisMaxPitchFemale = config.Bind("4. Synthesis", "Max Pitch Female", 1.25f,
                                                  new ConfigDescription("Highest possible pitch (relative percent) for female voices."));
        
        SynthesisMinPitchNonBinary = config.Bind("4. Synthesis", "Min Pitch Non-Binary", 0.75f,
                                                   new ConfigDescription("Lowest possible pitch (relative percent) for non-binary voices."));
        
        SynthesisMaxPitchNonBinary = config.Bind("4. Synthesis", "Max Pitch Non-Binary", 1.25f,
                                                   new ConfigDescription("Highest possible pitch (relative percent) for non-binary voices."));

        Utilities.EnforceMinMax(ref SynthesisMinSpeed, ref SynthesisMaxSpeed);
        Utilities.EnforceMinMax(ref SynthesisMinPitchMale, ref SynthesisMaxPitchMale);
        Utilities.EnforceMinMax(ref SynthesisMinPitchFemale, ref SynthesisMaxPitchFemale);
        Utilities.EnforceMinMax(ref SynthesisMinPitchNonBinary, ref SynthesisMaxPitchNonBinary);
    }

    private static void ResetSynthesis()
    {
        SynthesisVoiceFilter.Value = (SynthesisVoiceFilterType)SynthesisVoiceFilter.DefaultValue;
        SynthesisVoiceFilterInput.Value = (string)SynthesisVoiceFilterInput.DefaultValue;
        SynthesisVoiceLocaleFilter.Value = (string)SynthesisVoiceLocaleFilter.DefaultValue;
        SynthesisDebugVoiceName.Value = (bool)SynthesisDebugVoiceName.DefaultValue;
        SynthesisMinSpeed.Value = (int)SynthesisMinSpeed.DefaultValue;
        SynthesisMaxSpeed.Value = (int)SynthesisMaxSpeed.DefaultValue;
        SynthesisMinPitchMale.Value = (float)SynthesisMinPitchMale.DefaultValue;
        SynthesisMaxPitchMale.Value = (float)SynthesisMaxPitchMale.DefaultValue;
        SynthesisMinPitchFemale.Value = (float)SynthesisMinPitchFemale.DefaultValue;
        SynthesisMaxPitchFemale.Value = (float)SynthesisMaxPitchFemale.DefaultValue;
        SynthesisMinPitchNonBinary.Value = (float)SynthesisMinPitchNonBinary.DefaultValue;
        SynthesisMaxPitchNonBinary.Value = (float)SynthesisMaxPitchNonBinary.DefaultValue;
    }
}