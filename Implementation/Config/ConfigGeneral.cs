using BepInEx.Configuration;
using BepInEx.Logging;
using Babbler.Implementation.Common;

namespace Babbler.Implementation.Config;

public static partial class BabblerConfig
{
    public static bool Enabled = true;
    public static SpeechMode Mode = SpeechMode.Synthesis;

    public static bool DistortPhoneSpeech = true;
    
    public static float ConversationalVolume = 0.7f;
    public static float OverheardVolume = 0.3f;
    public static float PhoneVolume = 0.5f;

    public static float ConversationalShoutMultiplier = 2.9f;
    public static float OverheardShoutMultiplier = 6.6f;
    public static float PhoneShoutMultiplier = 4f;

    public static float FemaleThreshold = 0.49f;
    public static float MaleThreshold = 0.51f;
    public static float GenderDiversity = 0.05f;
    
    public static void Initialize(ConfigFile config)
    {
        Enabled = config.Bind("General", "Enabled", true,
                              new ConfigDescription("Another method of enabling and disabling Babbler.")).Value;
        
        Mode = config.Bind("General", "Mode", SpeechMode.Synthesis,
                           new ConfigDescription("Determines whether citizens will talk with text to speech synthesis, phonetic sounds, or monosyllabic droning.")).Value;

        DistortPhoneSpeech = config.Bind("General", "Distort Phone Speech", true,
                                         new ConfigDescription("When enabled, a band pass is applied to phones to make them sound a little tinnier, like phones.")).Value;

        ConversationalVolume = config.Bind("Volume", "Conversational Volume", 0.7f,
                                           new ConfigDescription("How loud voices will be when you are speaking directly to a person.")).Value;
        
        OverheardVolume = config.Bind("Volume", "Overheard Volume", 0.3f,
                                      new ConfigDescription("How loud voices that you overhear nearby will be when you are not talking directly to them.")).Value;

        PhoneVolume = config.Bind("Volume", "Phone Volume", 0.5f,
                                  new ConfigDescription("How loud voices will be when you are talking with a person over the phone.")).Value;
        
        ConversationalShoutMultiplier = config.Bind("Volume", "Conversational Shout Multiplier", 2.9f,
                                                    new ConfigDescription("When speaking in all caps, how much to multiply the normal conversational volume.")).Value;
        
        OverheardShoutMultiplier = config.Bind("Volume", "Overheard Shout Multiplier", 6.6f,
                                               new ConfigDescription("When speaking in all caps, how much to multiply the normal overheard volume.")).Value;
        
        PhoneShoutMultiplier = config.Bind("Volume", "Phone Shout Multiplier", 4f,
                                           new ConfigDescription("When speaking in all caps, how much to multiply the normal phone volume.")).Value;
        
        FemaleThreshold = config.Bind("Gender", "Female Threshold", 0.49f,
                                      new ConfigDescription("Increase for more female voices, decrease for less, defaults to what the stock game uses for citizens.",
                                                            new AcceptableValueRange<float>(0f, 1f))).Value;
        
        MaleThreshold = config.Bind("Gender", "Male Threshold", 0.51f,
                                    new ConfigDescription("Decrease for more male voices, increase for less, defaults to what the stock game uses for citizens.",
                                                          new AcceptableValueRange<float>(0f, 1f))).Value;

        GenderDiversity = config.Bind("Gender", "Gender Diversity", 0.05f,
                                      new ConfigDescription("Adds a random element to voice gender selection, increase for more diverse voices.",
                                                            new AcceptableValueRange<float>(0f, 1f))).Value;
        
        InitializeSynthesis(config);
        InitializePhonetic(config);
        InitializeDroning(config);

        Utilities.Log("BabblerConfig has initialized!", LogLevel.Debug);
    }
}