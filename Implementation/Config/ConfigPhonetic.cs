using BepInEx.Configuration;
using UnityEngine;

namespace Babbler.Implementation.Config;

public static partial class BabblerConfig
{
    public static bool UseMonosyllabicPhonetics = false;
    public static string ValidMonosyllables = "aeiouybdglmptvw";

    public static float BaseSyllableDelay = -0.175f;
    public static float MinimumSyllableDelayVariance = 0f;
    public static float MaximumSyllableDelayVariance = 0.4f;

    public static float MinimumSyllablePitchVariance = 0.8f;
    public static float MaximumSyllablePitchVariance = 1.2f;
    
    public static float PhoneticFrequencyMaleMinimum = 100f;
    public static float PhoneticFrequencyMaleMaximum = 180f;
    
    public static float PhoneticFrequencyFemaleMinimum = 165f;
    public static float PhoneticFrequencyFemaleMaximum = 255f;
    
    public static float PhoneticFrequencyNonBinaryMinimum = 100f;
    public static float PhoneticFrequencyNonBinaryMaximum = 255f;

    private static void InitializePhonetic(ConfigFile config)
    {
        UseMonosyllabicPhonetics = config.Bind("Phonetic", "Use Monosyllabic Phonetics", false,
                                    new ConfigDescription("Citizens will use a single repeating (but random) syllable for all phonetic speech.")).Value;
        
        ValidMonosyllables = config.Bind("Phonetic", "Valid Monosyllables", "aeiouybdglmptvw",
                                         new ConfigDescription("When using monosyllabic phonetics, citizens will choose from these syllables.")).Value;
        
        BaseSyllableDelay = config.Bind("Phonetic", "Base Syllable Delay", -0.175f,
                                        new ConfigDescription("The delay of each syllable is its length plus this. Negative numbers cause overlapping syllables.")).Value;
        
        MinimumSyllableDelayVariance = config.Bind("Phonetic", "Minimum Syllable Delay Variance", 0f,
                                                   new ConfigDescription("A random value between minimum and maximum syllable delay variance is added to the base delay to create more random delays between syllables.")).Value;
        
        MaximumSyllableDelayVariance = config.Bind("Phonetic", "Maximum Syllable Delay Variance", 0.4f,
                                                   new ConfigDescription("A random value between minimum and maximum syllable delay variance is added to the base delay to create more random delays between syllables.")).Value;
        
        MinimumSyllablePitchVariance = config.Bind("Phonetic", "Minimum Syllable Pitch Variance", 0.8f,
                                                   new ConfigDescription("A random value between minimum and maximum syllable pitch variance is multiplied against the syllable pitch to make individual syllables sound different.")).Value;
        
        MaximumSyllablePitchVariance = config.Bind("Phonetic", "Maximum Syllable Pitch Variance", 1.2f,
                                                   new ConfigDescription("A random value between minimum and maximum syllable pitch variance is multiplied against the syllable pitch to make individual syllables sound different.")).Value;
        
        PhoneticFrequencyMaleMinimum = config.Bind("Phonetic", "Phonetic Frequency Male Minimum", 100f,
                                             new ConfigDescription("Lowest possible frequency (in hertz) for male voices in Phonetic mode.")).Value;
        
        PhoneticFrequencyMaleMaximum = config.Bind("Phonetic", "Phonetic Frequency Male Maximum", 180f,
                                             new ConfigDescription("Highest possible frequency (in hertz) for male voices in Phonetic mode.")).Value;
        
        PhoneticFrequencyFemaleMinimum = config.Bind("Phonetic", "Phonetic Frequency Female Minimum", 165f,
                                               new ConfigDescription("Lowest possible frequency (in hertz) for female voices in Phonetic mode.")).Value;
        
        PhoneticFrequencyFemaleMaximum = config.Bind("Phonetic", "Phonetic Frequency Female Maximum", 255f,
                                               new ConfigDescription("Highest possible frequency (in hertz) for female voices in Phonetic mode.")).Value;
        
        PhoneticFrequencyNonBinaryMinimum = config.Bind("Phonetic", "Phonetic Frequency Non-Binary Minimum", 100f,
                                                new ConfigDescription("Lowest possible frequency (in hertz) for non-binary voices in Phonetic mode.")).Value;
        
        PhoneticFrequencyNonBinaryMaximum = config.Bind("Phonetic", "Phonetic Frequency Non-Binary Maximum", 255f,
                                                new ConfigDescription("Highest possible frequency (in hertz) for non-binary voices in Phonetic mode.")).Value;

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