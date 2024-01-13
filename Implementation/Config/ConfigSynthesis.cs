using BepInEx.Configuration;

namespace Babbler.Implementation.Config;

public static partial class BabblerConfig
{
    public static float SynthesisPitchMaleMinimum = 0.5f;
    public static float SynthesisPitchMaleMaximum = 1.3f;
    
    public static float SynthesisPitchFemaleMinimum = 0.7f;
    public static float SynthesisPitchFemaleMaximum = 1.5f;
    
    public static float SynthesisPitchNonBinaryMinimum = 0.5f;
    public static float SynthesisPitchNonBinaryMaximum = 1.5f;

    private static void InitializeSynthesis(ConfigFile config)
    {
        SynthesisPitchMaleMinimum = config.Bind("Synthesis Pitch", "Synthesis Pitch Male Minimum", 0.5f,
                                                new ConfigDescription("Lowest possible pitch for male voices in Synthesis mode.")).Value;
        
        SynthesisPitchMaleMaximum = config.Bind("Synthesis Pitch", "Synthesis Pitch Male Maximum", 1.3f,
                                                new ConfigDescription("Highest possible pitch for male voices in Synthesis mode.")).Value;
        
        SynthesisPitchFemaleMinimum = config.Bind("Synthesis Pitch", "Synthesis Pitch Female Minimum", 0.7f,
                                                  new ConfigDescription("Lowest possible pitch for female voices in Synthesis mode.")).Value;
        
        SynthesisPitchFemaleMaximum = config.Bind("Synthesis Pitch", "Synthesis Pitch Female Maximum", 1.5f,
                                                  new ConfigDescription("Highest possible pitch for female voices in Synthesis mode.")).Value;
        
        SynthesisPitchNonBinaryMinimum = config.Bind("Synthesis Pitch", "Synthesis Pitch Non-Binary Minimum", 0.5f,
                                                   new ConfigDescription("Lowest possible pitch for non-binary voices in Synthesis mode.")).Value;
        
        SynthesisPitchNonBinaryMaximum = config.Bind("Synthesis Pitch", "Synthesis Pitch Non-Binary Maximum", 1.5f,
                                                   new ConfigDescription("Highest possible pitch for non-binary voices in Synthesis mode.")).Value;
    }
}