﻿using BepInEx.Configuration;
using BepInEx.Logging;
using Babbler.Implementation.Common;

namespace Babbler.Implementation.Config;

public static partial class BabblerConfig
{
    public static ConfigEntry<bool> Enabled;
    public static ConfigEntry<SpeechMode> Mode;

    public static ConfigEntry<bool> DistortPhoneSpeech;

    public static ConfigEntry<float> ConversationalVolume;
    public static ConfigEntry<float> OverheardVolume;
    public static ConfigEntry<float> PhoneVolume;

    public static ConfigEntry<float> ConversationalShoutMultiplier;
    public static ConfigEntry<float> OverheardShoutMultiplier;
    public static ConfigEntry<float> PhoneShoutMultiplier;

    public static ConfigEntry<float> FemaleThreshold;
    public static ConfigEntry<float> MaleThreshold;
    public static ConfigEntry<float> GenderDiversity;
    
    public static void Initialize(ConfigFile config)
    {
        Enabled = config.Bind("1. General", "Enabled", true,
                              new ConfigDescription("Another method of enabling and disabling Babbler."));
        
        Mode = config.Bind("1. General", "Mode", SpeechMode.Synthesis,
                           new ConfigDescription("Determines whether citizens will talk with text to speech synthesis, phonetic sounds, or monosyllabic droning."));

        DistortPhoneSpeech = config.Bind("1. General", "Distort Phone Speech", true,
                                         new ConfigDescription("When enabled, a band pass is applied to phones to make them sound a little tinnier, like phones."));

        FemaleThreshold = config.Bind("2. Gender", "Female Threshold", 0.49f,
                                      new ConfigDescription("Increase for more female voices, decrease for less, defaults to what the stock game uses for citizens.",
                                                            new AcceptableValueRange<float>(0f, 1f)));
        
        MaleThreshold = config.Bind("2. Gender", "Male Threshold", 0.51f,
                                    new ConfigDescription("Decrease for more male voices, increase for less, defaults to what the stock game uses for citizens.",
                                                          new AcceptableValueRange<float>(0f, 1f)));

        GenderDiversity = config.Bind("2. Gender", "Gender Diversity", 0.05f,
                                      new ConfigDescription("Adds a random element to voice gender selection, increase for more diverse voices.",
                                                            new AcceptableValueRange<float>(0f, 1f)));
        
        ConversationalVolume = config.Bind("3. Volume", "Conversational Volume", 0.7f,
                                           new ConfigDescription("How loud voices will be when you are speaking directly to a person."));
        
        OverheardVolume = config.Bind("3. Volume", "Overheard Volume", 0.3f,
                                      new ConfigDescription("How loud voices that you overhear nearby will be when you are not talking directly to them."));

        PhoneVolume = config.Bind("3. Volume", "Phone Volume", 0.5f,
                                  new ConfigDescription("How loud voices will be when you are talking with a person over the phone."));
        
        ConversationalShoutMultiplier = config.Bind("3. Volume", "Conversational Shout Multiplier", 2.9f,
                                                    new ConfigDescription("When speaking in all caps, how much to multiply the normal conversational volume."));
        
        OverheardShoutMultiplier = config.Bind("3. Volume", "Overheard Shout Multiplier", 6.6f,
                                               new ConfigDescription("When speaking in all caps, how much to multiply the normal overheard volume."));
        
        PhoneShoutMultiplier = config.Bind("3. Volume", "Phone Shout Multiplier", 4f,
                                           new ConfigDescription("When speaking in all caps, how much to multiply the normal phone volume."));
        
        InitializeSynthesis(config);
        InitializePhonetic(config);
        InitializeDroning(config);

        Utilities.Log("BabblerConfig has initialized!", LogLevel.Debug);
    }
}