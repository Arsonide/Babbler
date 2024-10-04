using BepInEx.Configuration;

namespace Babbler.Implementation.Config;

public static partial class BabblerConfig
{
    public static ConfigEntry<float> ConversationalVolume;
    public static ConfigEntry<float> ConversationalShoutVolume;
    public static ConfigEntry<float> ConversationalEmoteVolume;

    public static ConfigEntry<float> OverheardVolume;
    public static ConfigEntry<float> OverheardShoutVolume;
    public static ConfigEntry<float> OverheardEmoteVolume;

    public static ConfigEntry<float> PhoneVolume;
    public static ConfigEntry<float> PhoneShoutVolume;
    public static ConfigEntry<float> PhoneEmoteVolume;

    public static ConfigEntry<float> OpenDoorOcclusionMultiplier;
    public static ConfigEntry<float> ClosedDoorOcclusionMultiplier;
    public static ConfigEntry<float> VentOcclusionMultiplier;
    public static ConfigEntry<float> DistantOcclusionMultiplier;

    public static ConfigEntry<bool> OcclusionEnabled;
    public static ConfigEntry<int> OcclusionNodeRange;
    public static ConfigEntry<int> OcclusionVentRange;

    private static void InitializeVolume(ConfigFile config)
    {
        ConversationalVolume = config.Bind("3. Volume", "Conversational Volume", 1.75f,
                                           new ConfigDescription("How loud voices will be when you are speaking directly to a person."));
        
        ConversationalShoutVolume = config.Bind("3. Volume", "Conversational Shout Volume", 5f,
                                           new ConfigDescription("How loud shouts in all caps will be when you are speaking directly to a person."));
        
        ConversationalEmoteVolume = config.Bind("3. Volume", "Conversational Emotes Volume", 1.75f,
                                                new ConfigDescription("How loud emote sound effects will be when you are speaking directly to a person."));
        
        OverheardVolume = config.Bind("3. Volume", "Overheard Volume", 0.75f,
                                      new ConfigDescription("How loud voices that you overhear nearby will be when you are not talking directly to them."));
        
        OverheardShoutVolume = config.Bind("3. Volume", "Overheard Shout Volume", 5f,
                                      new ConfigDescription("How loud shouts in all caps that you overhear nearby will be when you are not talking directly to them."));
        
        OverheardEmoteVolume = config.Bind("3. Volume", "Overheard Emotes Volume", 1.25f,
                                           new ConfigDescription("How loud emote sound effects that you overhear nearby will be when you are not talking directly to them."));

        PhoneVolume = config.Bind("3. Volume", "Phone Volume", 1.25f,
                                  new ConfigDescription("How loud voices will be when you are talking with a person over the phone."));
        
        PhoneShoutVolume = config.Bind("3. Volume", "Phone Shout Volume", 5f,
                                  new ConfigDescription("How loud shouts in all caps will be when you are talking with a person over the phone."));
        
        PhoneEmoteVolume = config.Bind("3. Volume", "Phone Emotes Volume", 1.5f,
                                  new ConfigDescription("How loud emote sound effects will be when you are talking with a person over the phone."));

        OpenDoorOcclusionMultiplier = config.Bind("3. Volume", "Open Door Occlusion Multiplier", 1f,
                                       new ConfigDescription("When sounds go through an open door, multiply their volume by this."));
        
        ClosedDoorOcclusionMultiplier = config.Bind("3. Volume", "Closed Door Occlusion Multiplier", 0.3f,
                                                  new ConfigDescription("When sounds go through a closed door, multiply their volume by this."));
        
        VentOcclusionMultiplier = config.Bind("3. Volume", "Vent Occlusion Multiplier", 0.6f,
                                                  new ConfigDescription("When sounds go through vent grating, multiply their volume by this."));
        
        DistantOcclusionMultiplier = config.Bind("3. Volume", "Distant Occlusion Multiplier", 0.1f,
                                                  new ConfigDescription("When sounds are audible but far away, multiply their volume by this."));
        
        OcclusionEnabled = config.Bind("3. Volume", "Occlusion Enabled", true,
                                         new ConfigDescription("Whether or not to process audio occlusion on Babbler sounds. Disabling will improve performance but is not advised as you will hear through walls."));
        
        OcclusionNodeRange = config.Bind("3. Volume", "Occlusion Node Range", 10,
                                                 new ConfigDescription("How many nodes away you hear sounds. Higher means more sounds, less performance. Lower means less sounds, more performance."));
        
        OcclusionVentRange = config.Bind("3. Volume", "Occlusion Vent Range", 10,
                                         new ConfigDescription("How many vent ducts sounds can go through. Higher means more sounds, less performance. Lower means less sounds, more performance."));
    }

    private static void ResetVolume()
    {
        ConversationalVolume.Value = (float)ConversationalVolume.DefaultValue;
        ConversationalShoutVolume.Value = (float)ConversationalShoutVolume.DefaultValue;
        ConversationalEmoteVolume.Value = (float)ConversationalEmoteVolume.DefaultValue;
        OverheardVolume.Value = (float)OverheardVolume.DefaultValue;
        OverheardShoutVolume.Value = (float)OverheardShoutVolume.DefaultValue;
        OverheardEmoteVolume.Value = (float)OverheardEmoteVolume.DefaultValue;
        PhoneVolume.Value = (float)PhoneVolume.DefaultValue;
        PhoneShoutVolume.Value = (float)PhoneShoutVolume.DefaultValue;
        PhoneEmoteVolume.Value = (float)PhoneEmoteVolume.DefaultValue;
        OpenDoorOcclusionMultiplier.Value = (float)OpenDoorOcclusionMultiplier.DefaultValue;
        ClosedDoorOcclusionMultiplier.Value = (float)ClosedDoorOcclusionMultiplier.DefaultValue;
        VentOcclusionMultiplier.Value = (float)VentOcclusionMultiplier.DefaultValue;
        DistantOcclusionMultiplier.Value = (float)DistantOcclusionMultiplier.DefaultValue;
        OcclusionEnabled.Value = (bool)OcclusionEnabled.DefaultValue;
        OcclusionNodeRange.Value = (int)OcclusionNodeRange.DefaultValue;
        OcclusionVentRange.Value = (int)OcclusionVentRange.DefaultValue;
    }
}