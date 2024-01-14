using BepInEx.Configuration;
using UnityEngine;

namespace Babbler.Implementation.Config;

public static partial class BabblerConfig
{
    public static bool UseMonosyllabicBlurbs = false;
    public static string ValidMonosyllables = "aeiouybdglmptvw";

    public static float BaseSyllableDelay = -0.2f;
    public static float MinimumSyllableDelayVariance = 0f;
    public static float MaximumSyllableDelayVariance = 0.4f;

    public static float MinimumSyllablePitchVariance = 0.8f;
    public static float MaximumSyllablePitchVariance = 1.2f;
    
    public static float BlurbsPitchMaleMinimum = 0.8f;
    public static float BlurbsPitchMaleMaximum = 1.2f;
    
    public static float BlurbsPitchFemaleMinimum = 0.8f;
    public static float BlurbsPitchFemaleMaximum = 1.2f;
    
    public static float BlurbsPitchNonBinaryMinimum = 0.8f;
    public static float BlurbsPitchNonBinaryMaximum = 1.2f;

    private static void InitializeBlurbs(ConfigFile config)
    {
        UseMonosyllabicBlurbs = config.Bind("Blurbs", "Use Monosyllabic Blurbs", false,
                                    new ConfigDescription("Citizens will use a single repeating (but random) syllable for all blurb speech.")).Value;
        
        ValidMonosyllables = config.Bind("Blurbs", "Valid Monosyllables", "aeiouybdglmptvw",
                                         new ConfigDescription("When using monosyllabic blurbs, citizens will choose from these syllables.")).Value;
        
        BaseSyllableDelay = config.Bind("Blurbs", "Base Syllable Delay", -0.2f,
                                        new ConfigDescription("The delay of each syllable is its length plus this. Negative numbers cause overlapping syllables.")).Value;
        
        MinimumSyllableDelayVariance = config.Bind("Blurbs", "Minimum Syllable Delay Variance", 0f,
                                                   new ConfigDescription("A random value between minimum and maximum syllable delay variance is added to the base delay to create more random delays between syllables.")).Value;
        
        MaximumSyllableDelayVariance = config.Bind("Blurbs", "Maximum Syllable Delay Variance", 0.4f,
                                                   new ConfigDescription("A random value between minimum and maximum syllable delay variance is added to the base delay to create more random delays between syllables.")).Value;
        
        MinimumSyllablePitchVariance = config.Bind("Blurbs", "Minimum Syllable Pitch Variance", 0.8f,
                                                   new ConfigDescription("A random value between minimum and maximum syllable pitch variance is multiplied against the syllable pitch to make individual syllables sound different.")).Value;
        
        MaximumSyllablePitchVariance = config.Bind("Blurbs", "Maximum Syllable Pitch Variance", 1.2f,
                                                   new ConfigDescription("A random value between minimum and maximum syllable pitch variance is multiplied against the syllable pitch to make individual syllables sound different.")).Value;
        
        BlurbsPitchMaleMinimum = config.Bind("Blurbs", "Blurbs Pitch Male Minimum", 0.8f,
                                             new ConfigDescription("Lowest possible pitch for male voices in Blurbs mode.")).Value;
        
        BlurbsPitchMaleMaximum = config.Bind("Blurbs", "Blurbs Pitch Male Maximum", 1.2f,
                                             new ConfigDescription("Highest possible pitch for male voices in Blurbs mode.")).Value;
        
        BlurbsPitchFemaleMinimum = config.Bind("Blurbs", "Blurbs Pitch Female Minimum", 0.8f,
                                               new ConfigDescription("Lowest possible pitch for female voices in Blurbs mode.")).Value;
        
        BlurbsPitchFemaleMaximum = config.Bind("Blurbs", "Blurbs Pitch Female Maximum", 1.2f,
                                               new ConfigDescription("Highest possible pitch for female voices in Blurbs mode.")).Value;
        
        BlurbsPitchNonBinaryMinimum = config.Bind("Blurbs", "Blurbs Pitch Non-Binary Minimum", 0.8f,
                                                new ConfigDescription("Lowest possible pitch for non-binary voices in Blurbs mode.")).Value;
        
        BlurbsPitchNonBinaryMaximum = config.Bind("Blurbs", "Blurbs Pitch Non-Binary Maximum", 1.2f,
                                                new ConfigDescription("Highest possible pitch for non-binary voices in Blurbs mode.")).Value;

        float minSyllableDelayVariance = Mathf.Min(MinimumSyllableDelayVariance, MaximumSyllableDelayVariance);
        float maxSyllableDelayVariance = Mathf.Max(MinimumSyllableDelayVariance, MaximumSyllableDelayVariance);
        MinimumSyllableDelayVariance = minSyllableDelayVariance;
        MaximumSyllableDelayVariance = maxSyllableDelayVariance;
        
        float minSyllablePitchVariance = Mathf.Min(MinimumSyllablePitchVariance, MaximumSyllablePitchVariance);
        float maxSyllablePitchVariance = Mathf.Max(MinimumSyllablePitchVariance, MaximumSyllablePitchVariance);
        MinimumSyllablePitchVariance = minSyllablePitchVariance;
        MaximumSyllablePitchVariance = maxSyllablePitchVariance;
    }
}