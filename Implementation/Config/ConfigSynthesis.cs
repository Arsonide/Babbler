using BepInEx.Configuration;
using Babbler.Implementation.Common;

namespace Babbler.Implementation.Config;

public static partial class BabblerConfig
{
    public static ConfigEntry<float> SynthesisMinPitchMale;
    public static ConfigEntry<float> SynthesisMaxPitchMale;

    public static ConfigEntry<float> SynthesisMinPitchFemale;
    public static ConfigEntry<float> SynthesisMaxPitchFemale;

    public static ConfigEntry<float> SynthesisMinPitchNonBinary;
    public static ConfigEntry<float> SynthesisMaxPitchNonBinary;

    private static void InitializeSynthesis(ConfigFile config)
    {
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
        
        Utilities.EnforceMinMax(ref SynthesisMinPitchMale, ref SynthesisMaxPitchMale);
        Utilities.EnforceMinMax(ref SynthesisMinPitchFemale, ref SynthesisMaxPitchFemale);
        Utilities.EnforceMinMax(ref SynthesisMinPitchNonBinary, ref SynthesisMaxPitchNonBinary);
    }
}