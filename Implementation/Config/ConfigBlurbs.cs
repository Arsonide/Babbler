﻿using BepInEx.Configuration;

namespace Babbler.Implementation.Config;

public static partial class BabblerConfig
{
    public static bool UseMonosyllabicBlurbs = false;
    public static string ValidMonosyllables = "aeiouybdglmptvw";
    public static float SyllableSpeed = 0.2f;

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
        
        SyllableSpeed = config.Bind("Blurbs", "Syllable Speed", 0.2f,
                                    new ConfigDescription("Determines the pauses between blurbs in a character's speech. Higher numbers make them talk slower.")).Value;
        
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
    }
}