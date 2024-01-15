using Babbler.Implementation.Common;
using BepInEx.Configuration;

namespace Babbler.Implementation.Config;

public static partial class BabblerConfig
{
    public static ConfigEntry<float> PhoneticSpeechDelay;
    
    public static ConfigEntry<float> PhoneticChanceDelayVariance;
    public static ConfigEntry<float> PhoneticMinDelayVariance;
    public static ConfigEntry<float> PhoneticMaxDelayVariance;

    public static ConfigEntry<float> PhoneticChancePitchVariance;
    public static ConfigEntry<float> PhoneticMinPitchVariance;
    public static ConfigEntry<float> PhoneticMaxPitchVariance;
    
    public static ConfigEntry<float> PhoneticMinFrequencyMale;
    public static ConfigEntry<float> PhoneticMaxFrequencyMale;
    
    public static ConfigEntry<float> PhoneticMinFrequencyFemale;
    public static ConfigEntry<float> PhoneticMaxFrequencyFemale;
    
    public static ConfigEntry<float> PhoneticMinFrequencyNonBinary;
    public static ConfigEntry<float> PhoneticMaxFrequencyNonBinary;

    private static void InitializePhonetic(ConfigFile config)
    {
        PhoneticSpeechDelay = config.Bind("5. Phonetic", "Speech Delay", -0.175f,
                                          new ConfigDescription("The delay of each phoneme is its length plus this. Negative numbers cause overlapping phonemes."));
        
        PhoneticChanceDelayVariance = config.Bind("5. Phonetic", "Chance Delay Variance", 0f,
                                                  new ConfigDescription("This is the chance for any citizen to speak with variations in their phoneme delay."));
        
        PhoneticMinDelayVariance = config.Bind("5. Phonetic", "Min Delay Variance", 0f,
                                               new ConfigDescription("A value between the min and max delay variance is chosen to add to the speech delay to create variations in it."));
        
        PhoneticMaxDelayVariance = config.Bind("5. Phonetic", "Max Delay Variance", 0f,
                                               new ConfigDescription("A value between the min and max delay variance is chosen to add to the speech delay to create variations in it."));
        
        PhoneticChancePitchVariance = config.Bind("5. Phonetic", "Chance Pitch Variance", 0.2f,
                                                  new ConfigDescription("This is the chance for any citizen to speak with variations in their phoneme pitch."));
        
        PhoneticMinPitchVariance = config.Bind("5. Phonetic", "Min Pitch Variance", 0.9f,
                                               new ConfigDescription("A value between the min and max pitch variance is chosen to multiply with the phoneme pitch to create variations of it."));
        
        PhoneticMaxPitchVariance = config.Bind("5. Phonetic", "Max Pitch Variance", 1.1f,
                                               new ConfigDescription("A value between the min and max pitch variance is chosen to multiply with the phoneme pitch to create variations of it."));

        PhoneticMinFrequencyMale = config.Bind("5. Phonetic", "Min Frequency Male", 100f,
                                               new ConfigDescription("Lowest possible frequency (in hertz) for male voices."));
        
        PhoneticMaxFrequencyMale = config.Bind("5. Phonetic", "Max Frequency Male", 180f,
                                             new ConfigDescription("Highest possible frequency (in hertz) for male voices."));
        
        PhoneticMinFrequencyFemale = config.Bind("5. Phonetic", "Min Frequency Female", 165f,
                                               new ConfigDescription("Lowest possible frequency (in hertz) for female voices."));
        
        PhoneticMaxFrequencyFemale = config.Bind("5. Phonetic", "Max Frequency Female", 255f,
                                               new ConfigDescription("Highest possible frequency (in hertz) for female voices."));
        
        PhoneticMinFrequencyNonBinary = config.Bind("5. Phonetic", "Min Frequency Non-Binary", 100f,
                                                new ConfigDescription("Lowest possible frequency (in hertz) for non-binary voices."));
        
        PhoneticMaxFrequencyNonBinary = config.Bind("5. Phonetic", "Max Frequency Non-Binary", 255f,
                                                new ConfigDescription("Highest possible frequency (in hertz) for non-binary voices."));
        
        Utilities.EnforceMinMax(ref PhoneticMinDelayVariance, ref PhoneticMaxDelayVariance);
        Utilities.EnforceMinMax(ref PhoneticMinPitchVariance, ref PhoneticMaxPitchVariance);
        Utilities.EnforceMinMax(ref PhoneticMinFrequencyMale, ref PhoneticMaxFrequencyMale);
        Utilities.EnforceMinMax(ref PhoneticMinFrequencyFemale, ref PhoneticMaxFrequencyFemale);
        Utilities.EnforceMinMax(ref PhoneticMinFrequencyNonBinary, ref PhoneticMaxFrequencyNonBinary);
    }

    public static void ResetPhonetic()
    {
        PhoneticSpeechDelay.Value = (float)PhoneticSpeechDelay.DefaultValue;
        PhoneticChanceDelayVariance.Value = (float)PhoneticChanceDelayVariance.DefaultValue;
        PhoneticMinDelayVariance.Value = (float)PhoneticMinDelayVariance.DefaultValue;
        PhoneticMaxDelayVariance.Value = (float)PhoneticMaxDelayVariance.DefaultValue;
        PhoneticChancePitchVariance.Value = (float)PhoneticChancePitchVariance.DefaultValue;
        PhoneticMinPitchVariance.Value = (float)PhoneticMinPitchVariance.DefaultValue;
        PhoneticMaxPitchVariance.Value = (float)PhoneticMaxPitchVariance.DefaultValue;
        PhoneticMinFrequencyMale.Value = (float)PhoneticMinFrequencyMale.DefaultValue;
        PhoneticMaxFrequencyMale.Value = (float)PhoneticMaxFrequencyMale.DefaultValue;
        PhoneticMinFrequencyFemale.Value = (float)PhoneticMinFrequencyFemale.DefaultValue;
        PhoneticMaxFrequencyFemale.Value = (float)PhoneticMaxFrequencyFemale.DefaultValue;
        PhoneticMinFrequencyNonBinary.Value = (float)PhoneticMinFrequencyNonBinary.DefaultValue;
        PhoneticMaxFrequencyNonBinary.Value = (float)PhoneticMaxFrequencyNonBinary.DefaultValue;
    }
}