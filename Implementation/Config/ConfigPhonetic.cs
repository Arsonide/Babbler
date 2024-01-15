using Babbler.Implementation.Common;
using BepInEx.Configuration;

namespace Babbler.Implementation.Config;

public static partial class BabblerConfig
{
    public static float PhoneticSpeechDelay = -0.175f;

    public static float PhoneticChanceDelayVariance = 0f;
    public static float PhoneticMinDelayVariance = 0f;
    public static float PhoneticMaxDelayVariance = 0f;

    public static float PhoneticChancePitchVariance = 0.2f;
    public static float PhoneticMinPitchVariance = 0.9f;
    public static float PhoneticMaxPitchVariance = 1.1f;
    
    public static float PhoneticMinFrequencyMale = 100f;
    public static float PhoneticMaxFrequencyMale = 180f;
    
    public static float PhoneticMinFrequencyFemale = 165f;
    public static float PhoneticMaxFrequencyFemale = 255f;
    
    public static float PhoneticMinFrequencyNonBinary = 100f;
    public static float PhoneticMaxFrequencyNonBinary = 255f;

    private static void InitializePhonetic(ConfigFile config)
    {
        PhoneticSpeechDelay = config.Bind("Phonetic", "Speech Delay", -0.175f,
                                          new ConfigDescription("The delay of each phoneme is its length plus this. Negative numbers cause overlapping phonemes.")).Value;
        
        PhoneticChanceDelayVariance = config.Bind("Phonetic", "Chance Delay Variance", 0f,
                                                  new ConfigDescription("This is the chance for any citizen to speak with variations in their phoneme delay.")).Value;
        
        PhoneticMinDelayVariance = config.Bind("Phonetic", "Min Delay Variance", 0f,
                                               new ConfigDescription("A value between the min and max delay variance is chosen to add to the speech delay to create variations in it.")).Value;
        
        PhoneticMaxDelayVariance = config.Bind("Phonetic", "Max Delay Variance", 0f,
                                               new ConfigDescription("A value between the min and max delay variance is chosen to add to the speech delay to create variations in it.")).Value;
        
        PhoneticChancePitchVariance = config.Bind("Phonetic", "Chance Pitch Variance", 0.2f,
                                                  new ConfigDescription("This is the chance for any citizen to speak with variations in their phoneme pitch.")).Value;
        
        PhoneticMinPitchVariance = config.Bind("Phonetic", "Min Pitch Variance", 0.9f,
                                               new ConfigDescription("A value between the min and max pitch variance is chosen to multiply with the phoneme pitch to create variations of it.")).Value;
        
        PhoneticMaxPitchVariance = config.Bind("Phonetic", "Max Pitch Variance", 1.1f,
                                               new ConfigDescription("A value between the min and max pitch variance is chosen to multiply with the phoneme pitch to create variations of it.")).Value;

        PhoneticMinFrequencyMale = config.Bind("Phonetic", "Min Frequency Male", 100f,
                                               new ConfigDescription("Lowest possible frequency (in hertz) for male voices.")).Value;
        
        PhoneticMaxFrequencyMale = config.Bind("Phonetic", "Max Frequency Male", 180f,
                                             new ConfigDescription("Highest possible frequency (in hertz) for male voices.")).Value;
        
        PhoneticMinFrequencyFemale = config.Bind("Phonetic", "Min Frequency Female", 165f,
                                               new ConfigDescription("Lowest possible frequency (in hertz) for female voices.")).Value;
        
        PhoneticMaxFrequencyFemale = config.Bind("Phonetic", "Max Frequency Female", 255f,
                                               new ConfigDescription("Highest possible frequency (in hertz) for female voices.")).Value;
        
        PhoneticMinFrequencyNonBinary = config.Bind("Phonetic", "Min Frequency Non-Binary", 100f,
                                                new ConfigDescription("Lowest possible frequency (in hertz) for non-binary voices.")).Value;
        
        PhoneticMaxFrequencyNonBinary = config.Bind("Phonetic", "Max Frequency Non-Binary", 255f,
                                                new ConfigDescription("Highest possible frequency (in hertz) for non-binary voices.")).Value;
        
        Utilities.EnforceMinMax(ref PhoneticMinDelayVariance, ref PhoneticMaxDelayVariance);
        Utilities.EnforceMinMax(ref PhoneticMinPitchVariance, ref PhoneticMaxPitchVariance);
        Utilities.EnforceMinMax(ref PhoneticMinFrequencyMale, ref PhoneticMaxFrequencyMale);
        Utilities.EnforceMinMax(ref PhoneticMinFrequencyFemale, ref PhoneticMaxFrequencyFemale);
        Utilities.EnforceMinMax(ref PhoneticMinFrequencyNonBinary, ref PhoneticMaxFrequencyNonBinary);
    }
}