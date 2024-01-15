using BepInEx.Configuration;
using UnityEngine;

namespace Babbler.Implementation.Config;

public static partial class BabblerConfig
{
    public static bool UseMonosyllabicPhonetics = false;
    public static string ValidMonosyllables = "aeiouybdglmptvw";

    public static float BaseSyllableDelay = -0.2f;
    public static float MinimumSyllableDelayVariance = 0f;
    public static float MaximumSyllableDelayVariance = 0.4f;

    public static float MinimumSyllablePitchVariance = 0.8f;
    public static float MaximumSyllablePitchVariance = 1.2f;
    
    public static float PhoneticPitchMaleMinimum = 0.8f;
    public static float PhoneticPitchMaleMaximum = 1.2f;
    
    public static float PhoneticPitchFemaleMinimum = 0.8f;
    public static float PhoneticPitchFemaleMaximum = 1.2f;
    
    public static float PhoneticPitchNonBinaryMinimum = 0.8f;
    public static float PhoneticPitchNonBinaryMaximum = 1.2f;

    private static void InitializePhonetic(ConfigFile config)
    {
        UseMonosyllabicPhonetics = config.Bind("Phonetic", "Use Monosyllabic Phonetics", false,
                                    new ConfigDescription("Citizens will use a single repeating (but random) syllable for all phonetic speech.")).Value;
        
        ValidMonosyllables = config.Bind("Phonetic", "Valid Monosyllables", "aeiouybdglmptvw",
                                         new ConfigDescription("When using monosyllabic phonetics, citizens will choose from these syllables.")).Value;
        
        BaseSyllableDelay = config.Bind("Phonetic", "Base Syllable Delay", -0.2f,
                                        new ConfigDescription("The delay of each syllable is its length plus this. Negative numbers cause overlapping syllables.")).Value;
        
        MinimumSyllableDelayVariance = config.Bind("Phonetic", "Minimum Syllable Delay Variance", 0f,
                                                   new ConfigDescription("A random value between minimum and maximum syllable delay variance is added to the base delay to create more random delays between syllables.")).Value;
        
        MaximumSyllableDelayVariance = config.Bind("Phonetic", "Maximum Syllable Delay Variance", 0.4f,
                                                   new ConfigDescription("A random value between minimum and maximum syllable delay variance is added to the base delay to create more random delays between syllables.")).Value;
        
        MinimumSyllablePitchVariance = config.Bind("Phonetic", "Minimum Syllable Pitch Variance", 0.8f,
                                                   new ConfigDescription("A random value between minimum and maximum syllable pitch variance is multiplied against the syllable pitch to make individual syllables sound different.")).Value;
        
        MaximumSyllablePitchVariance = config.Bind("Phonetic", "Maximum Syllable Pitch Variance", 1.2f,
                                                   new ConfigDescription("A random value between minimum and maximum syllable pitch variance is multiplied against the syllable pitch to make individual syllables sound different.")).Value;
        
        PhoneticPitchMaleMinimum = config.Bind("Phonetic", "Phonetic Pitch Male Minimum", 0.8f,
                                             new ConfigDescription("Lowest possible pitch for male voices in Phonetic mode.")).Value;
        
        PhoneticPitchMaleMaximum = config.Bind("Phonetic", "Phonetic Pitch Male Maximum", 1.2f,
                                             new ConfigDescription("Highest possible pitch for male voices in Phonetic mode.")).Value;
        
        PhoneticPitchFemaleMinimum = config.Bind("Phonetic", "Phonetic Pitch Female Minimum", 0.8f,
                                               new ConfigDescription("Lowest possible pitch for female voices in Phonetic mode.")).Value;
        
        PhoneticPitchFemaleMaximum = config.Bind("Phonetic", "Phonetic Pitch Female Maximum", 1.2f,
                                               new ConfigDescription("Highest possible pitch for female voices in Phonetic mode.")).Value;
        
        PhoneticPitchNonBinaryMinimum = config.Bind("Phonetic", "Phonetic Pitch Non-Binary Minimum", 0.8f,
                                                new ConfigDescription("Lowest possible pitch for non-binary voices in Phonetic mode.")).Value;
        
        PhoneticPitchNonBinaryMaximum = config.Bind("Phonetic", "Phonetic Pitch Non-Binary Maximum", 1.2f,
                                                new ConfigDescription("Highest possible pitch for non-binary voices in Phonetic mode.")).Value;

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