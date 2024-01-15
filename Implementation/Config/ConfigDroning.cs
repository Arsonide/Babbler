using Babbler.Implementation.Common;
using BepInEx.Configuration;

namespace Babbler.Implementation.Config;

public static partial class BabblerConfig
{
    public static string DroningValidPhonemes = "aeiouybdglmptvw";

    public static float DroningSpeechDelay = -0.175f;

    public static float DroningChanceDelayVariance = 0.4f;
    public static float DroningMinDelayVariance = 0f;
    public static float DroningMaxDelayVariance = 0.3f;

    public static float DroningChancePitchVariance = 0.2f;
    public static float DroningMinPitchVariance = 0.8f;
    public static float DroningMaxPitchVariance = 1.2f;
    
    public static float DroningMinFrequencyMale = 100f;
    public static float DroningMaxFrequencyMale = 180f;
    
    public static float DroningMinFrequencyFemale = 165f;
    public static float DroningMaxFrequencyFemale = 255f;
    
    public static float DroningMinFrequencyNonBinary = 100f;
    public static float DroningMaxFrequencyNonBinary = 255f;

    private static void InitializeDroning(ConfigFile config)
    {
        DroningValidPhonemes = config.Bind("6. Droning", "Valid Phonemes", "aeiouybdglmptvw",
                                           new ConfigDescription("Citizens pick one phoneme to repeat, that is chosen from these phonemes.")).Value;
        
        DroningSpeechDelay = config.Bind("6. Droning", "Speech Delay", -0.175f,
                                          new ConfigDescription("The delay of each phoneme is its length plus this. Negative numbers cause overlapping phonemes.")).Value;
        
        DroningChanceDelayVariance = config.Bind("6. Droning", "Chance Delay Variance", 0.4f,
                                                  new ConfigDescription("This is the chance for any citizen to speak with variations in their phoneme delay.")).Value;
        
        DroningMinDelayVariance = config.Bind("6. Droning", "Min Delay Variance", 0f,
                                               new ConfigDescription("A value between the min and max delay variance is chosen to add to the speech delay to create variations in it.")).Value;
        
        DroningMaxDelayVariance = config.Bind("6. Droning", "Max Delay Variance", 0.3f,
                                               new ConfigDescription("A value between the min and max delay variance is chosen to add to the speech delay to create variations in it.")).Value;
        
        DroningChancePitchVariance = config.Bind("6. Droning", "Chance Pitch Variance", 0.2f,
                                                  new ConfigDescription("This is the chance for any citizen to speak with variations in their phoneme pitch.")).Value;
        
        DroningMinPitchVariance = config.Bind("6. Droning", "Min Pitch Variance", 0.8f,
                                               new ConfigDescription("A value between the min and max pitch variance is chosen to multiply with the phoneme pitch to create variations of it.")).Value;
        
        DroningMaxPitchVariance = config.Bind("6. Droning", "Max Pitch Variance", 1.2f,
                                               new ConfigDescription("A value between the min and max pitch variance is chosen to multiply with the phoneme pitch to create variations of it.")).Value;

        DroningMinFrequencyMale = config.Bind("6. Droning", "Min Frequency Male", 100f,
                                               new ConfigDescription("Lowest possible frequency (in hertz) for male voices.")).Value;
        
        DroningMaxFrequencyMale = config.Bind("6. Droning", "Max Frequency Male", 180f,
                                             new ConfigDescription("Highest possible frequency (in hertz) for male voices.")).Value;
        
        DroningMinFrequencyFemale = config.Bind("6. Droning", "Min Frequency Female", 165f,
                                               new ConfigDescription("Lowest possible frequency (in hertz) for female voices.")).Value;
        
        DroningMaxFrequencyFemale = config.Bind("6. Droning", "Max Frequency Female", 255f,
                                               new ConfigDescription("Highest possible frequency (in hertz) for female voices.")).Value;
        
        DroningMinFrequencyNonBinary = config.Bind("6. Droning", "Min Frequency Non-Binary", 100f,
                                                new ConfigDescription("Lowest possible frequency (in hertz) for non-binary voices.")).Value;
        
        DroningMaxFrequencyNonBinary = config.Bind("6. Droning", "Max Frequency Non-Binary", 255f,
                                                new ConfigDescription("Highest possible frequency (in hertz) for non-binary voices.")).Value;

        Utilities.EnforceMinMax(ref DroningMinDelayVariance, ref DroningMaxDelayVariance);
        Utilities.EnforceMinMax(ref DroningMinPitchVariance, ref DroningMaxPitchVariance);
        Utilities.EnforceMinMax(ref DroningMinFrequencyMale, ref DroningMaxFrequencyMale);
        Utilities.EnforceMinMax(ref DroningMinFrequencyFemale, ref DroningMaxFrequencyFemale);
        Utilities.EnforceMinMax(ref DroningMinFrequencyNonBinary, ref DroningMaxFrequencyNonBinary);
    }
}