using BepInEx.Configuration;
using Babbler.Implementation.Common;

namespace Babbler.Implementation.Config;

public static partial class BabblerConfig
{
    public static ConfigEntry<bool> IncidentalsEnabled;
    public static ConfigEntry<float> IncidentalsRange;
    public static ConfigEntry<float> IncidentalsMinDrunkForHiccups;
    public static ConfigEntry<float> IncidentalsMinBurpChance;
    public static ConfigEntry<float> IncidentalsMaxBurpChance;
    public static ConfigEntry<float> IncidentalsMinFartChance;
    public static ConfigEntry<float> IncidentalsMaxFartChance;
    public static ConfigEntry<float> IncidentalsMinHiccupChance;
    public static ConfigEntry<float> IncidentalsMaxHiccupChance;
    public static ConfigEntry<float> IncidentalsMinWhistleChance;
    public static ConfigEntry<float> IncidentalsMaxWhistleChance;

    private static void InitializeIncidentals(ConfigFile config)
    {
        IncidentalsEnabled = config.Bind("8. Incidentals", "Enabled", true,
                                              new ConfigDescription("Use random \"incidental\" emotes, that are not tied to actual dialog. (Like burps, farts, and hiccups.)"));
        
        IncidentalsRange = config.Bind("8. Incidentals", "Range", 25f,
                                            new ConfigDescription("How far away you can hear incidental emotes sound effects, in meters."));
        
        IncidentalsMinDrunkForHiccups = config.Bind("8. Incidentals", "Min Drunk For Hiccups", 0.25f,
                                                    new ConfigDescription("The lowest amount an NPC can be drunk before they start hiccuping.",
                                                                          new AcceptableValueRange<float>(0f, 1f)));
        
        IncidentalsMinBurpChance = config.Bind("8. Incidentals", "Min Burp Chance", 0f,
                                                 new ConfigDescription("The minimum chance for NPCs to burp when they finish eating. Set min and max to zero to disable burps specifically.",
                                                                       new AcceptableValueRange<float>(0f, 1f)));

        IncidentalsMaxBurpChance = config.Bind("8. Incidentals", "Max Burp Chance", 0.4f,
                                               new ConfigDescription("The maximum chance for NPCs to burp when they finish eating. Set min and max to zero to disable burps specifically.",
                                                                     new AcceptableValueRange<float>(0f, 1f)));
        
        IncidentalsMinFartChance = config.Bind("8. Incidentals", "Min Fart Chance", 0f,
                                               new ConfigDescription("The minimum chance for NPCs to fart when performing bathroom functions. Set min and max to zero to disable burps specifically.",
                                                                     new AcceptableValueRange<float>(0f, 1f)));
        
        IncidentalsMaxFartChance = config.Bind("8. Incidentals", "Max Fart Chance", 0.4f,
                                               new ConfigDescription("The maximum chance for NPCs to fart when performing bathroom functions. Set min and max to zero to disable burps specifically.",
                                                                     new AcceptableValueRange<float>(0f, 1f)));

        IncidentalsMinHiccupChance = config.Bind("8. Incidentals", "Min Hiccup Chance", 0f,
                                                 new ConfigDescription("The minimum chance for NPCs to hiccup as they walk drunk - smaller because it is evaluated more often. Set min and max to zero to disable hiccups specifically.",
                                                                       new AcceptableValueRange<float>(0f, 1f)));

        IncidentalsMaxHiccupChance = config.Bind("8. Incidentals", "Max Hiccup Chance", 0.25f,
                                                 new ConfigDescription("The maximum chance for NPCs to hiccup as they walk drunk - smaller because it is evaluated more often. Set min and max to zero to disable hiccups specifically.",
                                                                       new AcceptableValueRange<float>(0f, 1f)));
        
        IncidentalsMinWhistleChance = config.Bind("8. Incidentals", "Min Whistle Chance", 0f,
                                                 new ConfigDescription("The minimum chance for NPCs to whistle while they shower. Set min and max to zero to disable whistling specifically.",
                                                                       new AcceptableValueRange<float>(0f, 1f)));

        IncidentalsMaxWhistleChance = config.Bind("8. Incidentals", "Max Whistle Chance", 0.4f,
                                                 new ConfigDescription("The maximum chance for NPCs to whistle while they shower. Set min and max to zero to disable whistling specifically.",
                                                                       new AcceptableValueRange<float>(0f, 1f)));
        
        Utilities.EnforceMinMax(ref IncidentalsMinBurpChance, ref IncidentalsMaxBurpChance);
        Utilities.EnforceMinMax(ref IncidentalsMinFartChance, ref IncidentalsMaxFartChance);
        Utilities.EnforceMinMax(ref IncidentalsMinHiccupChance, ref IncidentalsMaxHiccupChance);
        Utilities.EnforceMinMax(ref IncidentalsMinWhistleChance, ref IncidentalsMaxWhistleChance);
    }

    private static void ResetIncidentals()
    {
        IncidentalsEnabled.Value = (bool)IncidentalsEnabled.DefaultValue;
        IncidentalsRange.Value = (float)IncidentalsRange.DefaultValue;
        IncidentalsMinDrunkForHiccups.Value = (float)IncidentalsMinDrunkForHiccups.DefaultValue;
        IncidentalsMinBurpChance.Value = (float)IncidentalsMinBurpChance.DefaultValue;
        IncidentalsMaxBurpChance.Value = (float)IncidentalsMaxBurpChance.DefaultValue;
        IncidentalsMinFartChance.Value = (float)IncidentalsMinFartChance.DefaultValue;
        IncidentalsMaxFartChance.Value = (float)IncidentalsMaxFartChance.DefaultValue;
        IncidentalsMinHiccupChance.Value = (float)IncidentalsMinHiccupChance.DefaultValue;
        IncidentalsMaxHiccupChance.Value = (float)IncidentalsMaxHiccupChance.DefaultValue;
        IncidentalsMinWhistleChance.Value = (float)IncidentalsMinWhistleChance.DefaultValue;
        IncidentalsMaxWhistleChance.Value = (float)IncidentalsMaxWhistleChance.DefaultValue;
    }
}