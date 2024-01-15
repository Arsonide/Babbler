using Babbler.Implementation.Common;
using BepInEx.Configuration;

namespace Babbler.Implementation.Config;

public static partial class BabblerConfig
{
    public static ConfigEntry<string> DroningValidPhonemes;

    public static ConfigEntry<float> DroningSpeechDelay;

    public static ConfigEntry<float> DroningChanceDelayVariance;
    public static ConfigEntry<float> DroningMinDelayVariance;
    public static ConfigEntry<float> DroningMaxDelayVariance;

    public static ConfigEntry<float> DroningChancePitchVariance;
    public static ConfigEntry<float> DroningMinPitchVariance;
    public static ConfigEntry<float> DroningMaxPitchVariance;
    
    public static ConfigEntry<float> DroningMinFrequencyMale;
    public static ConfigEntry<float> DroningMaxFrequencyMale;
    
    public static ConfigEntry<float> DroningMinFrequencyFemale;
    public static ConfigEntry<float> DroningMaxFrequencyFemale;
    
    public static ConfigEntry<float> DroningMinFrequencyNonBinary;
    public static ConfigEntry<float> DroningMaxFrequencyNonBinary;

    private static void InitializeDroning(ConfigFile config)
    {
        DroningValidPhonemes = config.Bind("6. Droning", "Valid Phonemes", "aeiouybdglmptvw",
                                           new ConfigDescription("Citizens pick one phoneme to repeat, that is chosen from these phonemes."));
        
        DroningSpeechDelay = config.Bind("6. Droning", "Speech Delay", -0.175f,
                                          new ConfigDescription("The delay of each phoneme is its length plus this. Negative numbers cause overlapping phonemes."));
        
        DroningChanceDelayVariance = config.Bind("6. Droning", "Chance Delay Variance", 0.4f,
                                                  new ConfigDescription("This is the chance for any citizen to speak with variations in their phoneme delay.",
                                                                        new AcceptableValueRange<float>(0f, 1f)));
        
        DroningMinDelayVariance = config.Bind("6. Droning", "Min Delay Variance", 0f,
                                               new ConfigDescription("A value between the min and max delay variance is chosen to add to the speech delay to create variations in it."));
        
        DroningMaxDelayVariance = config.Bind("6. Droning", "Max Delay Variance", 0.3f,
                                               new ConfigDescription("A value between the min and max delay variance is chosen to add to the speech delay to create variations in it."));
        
        DroningChancePitchVariance = config.Bind("6. Droning", "Chance Pitch Variance", 0.2f,
                                                  new ConfigDescription("This is the chance for any citizen to speak with variations in their phoneme pitch.",
                                                                        new AcceptableValueRange<float>(0f, 1f)));
        
        DroningMinPitchVariance = config.Bind("6. Droning", "Min Pitch Variance", 0.8f,
                                               new ConfigDescription("A value between the min and max pitch variance is chosen to multiply with the phoneme pitch to create variations of it."));
        
        DroningMaxPitchVariance = config.Bind("6. Droning", "Max Pitch Variance", 1.2f,
                                               new ConfigDescription("A value between the min and max pitch variance is chosen to multiply with the phoneme pitch to create variations of it."));

        DroningMinFrequencyMale = config.Bind("6. Droning", "Min Frequency Male", 100f,
                                               new ConfigDescription("Lowest possible frequency (in hertz) for male voices."));
        
        DroningMaxFrequencyMale = config.Bind("6. Droning", "Max Frequency Male", 180f,
                                             new ConfigDescription("Highest possible frequency (in hertz) for male voices."));
        
        DroningMinFrequencyFemale = config.Bind("6. Droning", "Min Frequency Female", 165f,
                                               new ConfigDescription("Lowest possible frequency (in hertz) for female voices."));
        
        DroningMaxFrequencyFemale = config.Bind("6. Droning", "Max Frequency Female", 255f,
                                               new ConfigDescription("Highest possible frequency (in hertz) for female voices."));
        
        DroningMinFrequencyNonBinary = config.Bind("6. Droning", "Min Frequency Non-Binary", 100f,
                                                new ConfigDescription("Lowest possible frequency (in hertz) for non-binary voices."));
        
        DroningMaxFrequencyNonBinary = config.Bind("6. Droning", "Max Frequency Non-Binary", 255f,
                                                new ConfigDescription("Highest possible frequency (in hertz) for non-binary voices."));

        Utilities.EnforceMinMax(ref DroningMinDelayVariance, ref DroningMaxDelayVariance);
        Utilities.EnforceMinMax(ref DroningMinPitchVariance, ref DroningMaxPitchVariance);
        Utilities.EnforceMinMax(ref DroningMinFrequencyMale, ref DroningMaxFrequencyMale);
        Utilities.EnforceMinMax(ref DroningMinFrequencyFemale, ref DroningMaxFrequencyFemale);
        Utilities.EnforceMinMax(ref DroningMinFrequencyNonBinary, ref DroningMaxFrequencyNonBinary);
    }

    public static void ResetDroning()
    {
        DroningValidPhonemes.Value = (string)DroningValidPhonemes.DefaultValue;
        DroningSpeechDelay.Value = (float)DroningSpeechDelay.DefaultValue;
        DroningChanceDelayVariance.Value = (float)DroningChanceDelayVariance.DefaultValue;
        DroningMinDelayVariance.Value = (float)DroningMinDelayVariance.DefaultValue;
        DroningMaxDelayVariance.Value = (float)DroningMaxDelayVariance.DefaultValue;
        DroningChancePitchVariance.Value = (float)DroningChancePitchVariance.DefaultValue;
        DroningMinPitchVariance.Value = (float)DroningMinPitchVariance.DefaultValue;
        DroningMaxPitchVariance.Value = (float)DroningMaxPitchVariance.DefaultValue;
        DroningMinFrequencyMale.Value = (float)DroningMinFrequencyMale.DefaultValue;
        DroningMaxFrequencyMale.Value = (float)DroningMaxFrequencyMale.DefaultValue;
        DroningMinFrequencyFemale.Value = (float)DroningMinFrequencyFemale.DefaultValue;
        DroningMaxFrequencyFemale.Value = (float)DroningMaxFrequencyFemale.DefaultValue;
        DroningMinFrequencyNonBinary.Value = (float)DroningMinFrequencyNonBinary.DefaultValue;
        DroningMaxFrequencyNonBinary.Value = (float)DroningMaxFrequencyNonBinary.DefaultValue;
    }
}