using BepInEx.Configuration;
using Babbler.Implementation.Common;

namespace Babbler.Implementation.Config;

public static partial class BabblerConfig
{
    public static float SynthesisMinPitchMale = 0.75f;
    public static float SynthesisMaxPitchMale = 1.25f;
    
    public static float SynthesisMinPitchFemale = 0.75f;
    public static float SynthesisMaxPitchFemale = 1.25f;
    
    public static float SynthesisMinPitchNonBinary = 0.75f;
    public static float SynthesisMaxPitchNonBinary = 1.25f;

    private static void InitializeSynthesis(ConfigFile config)
    {
        SynthesisMinPitchMale = config.Bind("Synthesis", "Min Pitch Male", 0.75f,
                                                new ConfigDescription("Lowest possible pitch (relative percent) for male voices.")).Value;
        
        SynthesisMaxPitchMale = config.Bind("Synthesis", "Max Pitch Male", 1.25f,
                                                new ConfigDescription("Highest possible pitch (relative percent) for male voices.")).Value;
        
        SynthesisMinPitchFemale = config.Bind("Synthesis", "Min Pitch Female", 0.75f,
                                                  new ConfigDescription("Lowest possible pitch (relative percent) for female voices.")).Value;
        
        SynthesisMaxPitchFemale = config.Bind("Synthesis", "Max Pitch Female", 1.25f,
                                                  new ConfigDescription("Highest possible pitch (relative percent) for female voices.")).Value;
        
        SynthesisMinPitchNonBinary = config.Bind("Synthesis", "Min Pitch Non-Binary", 0.75f,
                                                   new ConfigDescription("Lowest possible pitch (relative percent) for non-binary voices.")).Value;
        
        SynthesisMaxPitchNonBinary = config.Bind("Synthesis", "Max Pitch Non-Binary", 1.25f,
                                                   new ConfigDescription("Highest possible pitch (relative percent) for non-binary voices.")).Value;
        
        Utilities.EnforceMinMax(ref SynthesisMinPitchMale, ref SynthesisMaxPitchMale);
        Utilities.EnforceMinMax(ref SynthesisMinPitchFemale, ref SynthesisMaxPitchFemale);
        Utilities.EnforceMinMax(ref SynthesisMinPitchNonBinary, ref SynthesisMaxPitchNonBinary);
    }
}