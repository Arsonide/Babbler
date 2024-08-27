using FMOD;
using BepInEx.Logging;
using Babbler.Implementation.Config;

namespace Babbler.Implementation.Common;

public static class FMODRegistry
{
    // All direct interactions with System are done through helper methods so we can log if there are errors.
    private static FMOD.System System;
    
    private static ChannelGroup ConversationalGroup;
    private static ChannelGroup OverheardGroup;
    private static ChannelGroup PhoneGroup;

    private static ChannelGroup ConversationalShoutGroup;
    private static ChannelGroup OverheardShoutGroup;
    private static ChannelGroup PhoneShoutGroup;
    
    private static ChannelGroup ConversationalEmoteGroup;
    private static ChannelGroup OverheardEmoteGroup;
    private static ChannelGroup PhoneEmoteGroup;
    
    public static void Initialize()
    {
        System = FMODUnity.RuntimeManager.CoreSystem;
        
        SetupConversationalGroup();
        SetupOverheardGroup();
        SetupPhoneGroup();
        
        Utilities.Log("FMODRegistry has initialized!", LogLevel.Debug);
    }
    
    private static void SetupConversationalGroup()
    {
        TryCreateChannelGroup("BabblerConversationalGroup", out ConversationalGroup);
        TryCreateChannelGroup("BabblerConversationalShoutGroup", out ConversationalShoutGroup);
        TryCreateChannelGroup("BabblerConversationalEmoteGroup", out ConversationalEmoteGroup);
        ConversationalGroup.setVolume(BabblerConfig.ConversationalVolume.Value);
        ConversationalShoutGroup.setVolume(BabblerConfig.ConversationalVolume.Value * BabblerConfig.ConversationalShoutMultiplier.Value);
        ConversationalEmoteGroup.setVolume(BabblerConfig.ConversationalEmoteVolume.Value);
    }

    private static void SetupOverheardGroup()
    {
        TryCreateChannelGroup("BabblerOverheardGroup", out OverheardGroup);
        TryCreateChannelGroup("BabblerOverheardShoutGroup", out OverheardShoutGroup);
        TryCreateChannelGroup("BabblerOverheardEmoteGroup", out OverheardEmoteGroup);
        OverheardGroup.setVolume(BabblerConfig.OverheardVolume.Value);
        OverheardShoutGroup.setVolume(BabblerConfig.OverheardVolume.Value * BabblerConfig.OverheardShoutMultiplier.Value);
        OverheardEmoteGroup.setVolume(BabblerConfig.OverheardEmoteVolume.Value);
    }

    private static void SetupPhoneGroup()
    {
        TryCreateChannelGroup("BabblerPhoneGroup", out PhoneGroup);
        TryCreateChannelGroup("BabblerPhoneShoutGroup", out PhoneShoutGroup);
        TryCreateChannelGroup("BabblerPhoneEmoteGroup", out PhoneEmoteGroup);
        PhoneGroup.setVolume(BabblerConfig.PhoneVolume.Value);
        PhoneShoutGroup.setVolume(BabblerConfig.PhoneVolume.Value * BabblerConfig.PhoneShoutMultiplier.Value);
        PhoneEmoteGroup.setVolume(BabblerConfig.PhoneEmoteVolume.Value);

        if (!BabblerConfig.DistortPhoneSpeech.Value)
        {
            return;
        }

        TryCreateDSP(DSP_TYPE.MULTIBAND_EQ, out DSP phoneDSP);

        phoneDSP.setParameterInt((int)DSP_MULTIBAND_EQ.A_FILTER, (int)DSP_MULTIBAND_EQ_FILTER_TYPE.HIGHPASS_48DB);
        phoneDSP.setParameterFloat((int)DSP_MULTIBAND_EQ.A_FREQUENCY, 300f);

        phoneDSP.setParameterInt((int)DSP_MULTIBAND_EQ.B_FILTER, (int)DSP_MULTIBAND_EQ_FILTER_TYPE.PEAKING);
        phoneDSP.setParameterFloat((int)DSP_MULTIBAND_EQ.B_FREQUENCY, 1700f);
        phoneDSP.setParameterFloat((int)DSP_MULTIBAND_EQ.B_Q, 1f);

        phoneDSP.setParameterInt((int)DSP_MULTIBAND_EQ.C_FILTER, (int)DSP_MULTIBAND_EQ_FILTER_TYPE.LOWPASS_48DB);
        phoneDSP.setParameterFloat((int)DSP_MULTIBAND_EQ.C_FREQUENCY, 3400f);

        PhoneGroup.addDSP(CHANNELCONTROL_DSP_INDEX.HEAD, phoneDSP);
        
        TryCreateDSP(DSP_TYPE.MULTIBAND_EQ, out DSP phoneShoutDSP);

        phoneShoutDSP.setParameterInt((int)DSP_MULTIBAND_EQ.A_FILTER, (int)DSP_MULTIBAND_EQ_FILTER_TYPE.HIGHPASS_48DB);
        phoneShoutDSP.setParameterFloat((int)DSP_MULTIBAND_EQ.A_FREQUENCY, 300f);

        phoneShoutDSP.setParameterInt((int)DSP_MULTIBAND_EQ.B_FILTER, (int)DSP_MULTIBAND_EQ_FILTER_TYPE.PEAKING);
        phoneShoutDSP.setParameterFloat((int)DSP_MULTIBAND_EQ.B_FREQUENCY, 1700f);
        phoneShoutDSP.setParameterFloat((int)DSP_MULTIBAND_EQ.B_Q, 1f);

        phoneShoutDSP.setParameterInt((int)DSP_MULTIBAND_EQ.C_FILTER, (int)DSP_MULTIBAND_EQ_FILTER_TYPE.LOWPASS_48DB);
        phoneShoutDSP.setParameterFloat((int)DSP_MULTIBAND_EQ.C_FREQUENCY, 3400f);
        
        PhoneShoutGroup.addDSP(CHANNELCONTROL_DSP_INDEX.HEAD, phoneShoutDSP);
        
        TryCreateDSP(DSP_TYPE.MULTIBAND_EQ, out DSP phoneEmoteDSP);

        phoneEmoteDSP.setParameterInt((int)DSP_MULTIBAND_EQ.A_FILTER, (int)DSP_MULTIBAND_EQ_FILTER_TYPE.HIGHPASS_48DB);
        phoneEmoteDSP.setParameterFloat((int)DSP_MULTIBAND_EQ.A_FREQUENCY, 300f);

        phoneEmoteDSP.setParameterInt((int)DSP_MULTIBAND_EQ.B_FILTER, (int)DSP_MULTIBAND_EQ_FILTER_TYPE.PEAKING);
        phoneEmoteDSP.setParameterFloat((int)DSP_MULTIBAND_EQ.B_FREQUENCY, 1700f);
        phoneEmoteDSP.setParameterFloat((int)DSP_MULTIBAND_EQ.B_Q, 1f);

        phoneEmoteDSP.setParameterInt((int)DSP_MULTIBAND_EQ.C_FILTER, (int)DSP_MULTIBAND_EQ_FILTER_TYPE.LOWPASS_48DB);
        phoneEmoteDSP.setParameterFloat((int)DSP_MULTIBAND_EQ.C_FREQUENCY, 3400f);

        PhoneEmoteGroup.addDSP(CHANNELCONTROL_DSP_INDEX.HEAD, phoneEmoteDSP);
    }

    public static ChannelGroup GetChannelGroup(SoundContext soundContext)
    {
        switch (soundContext)
        {
            case SoundContext.ConversationalSpeech:
                return ConversationalGroup;
            case SoundContext.OverheardSpeech:
                return OverheardGroup;
            case SoundContext.PhoneSpeech:
                return PhoneGroup;
            case SoundContext.ConversationalShout:
                return ConversationalShoutGroup;
            case SoundContext.OverheardShout:
                return OverheardShoutGroup;
            case SoundContext.PhoneShout:
                return PhoneShoutGroup;
            case SoundContext.ConversationalEmote:
                return ConversationalEmoteGroup;
            case SoundContext.OverheardEmote:
                return OverheardEmoteGroup;
            case SoundContext.PhoneEmote:
                return PhoneEmoteGroup;
            default:
                return OverheardGroup;
        }
    }
    
#region System Interactions
    
    private static bool TryCreateChannelGroup(string name, out ChannelGroup channelGroup)
    {
        RESULT result = System.createChannelGroup("BabblerSpeechGroup", out channelGroup);

        if (result == RESULT.OK)
        {
            return true;
        }

        Utilities.Log($"FMODRegistry failed to create channel group \"{name}\" at {Utilities.GetCallingMethodName()}. Error: {result.ToString()}", LogLevel.Error);
        return false;
    }

    private static bool TryCreateDSP(DSP_TYPE dspType, out DSP dsp)
    {
        RESULT result = System.createDSPByType(dspType, out dsp);

        if (result == RESULT.OK)
        {
            return true;
        }

        Utilities.Log($"FMODRegistry failed to create DSP \"{dspType.ToString()}\" at {Utilities.GetCallingMethodName()}. Error: {result.ToString()}", LogLevel.Error);
        return false;
    }
    
    public static bool TryCreateSound(string name, MODE mode, out Sound sound)
    {
        RESULT result = System.createSound(name, mode, out sound);

        if (result == RESULT.OK)
        {
            return true;
        }

        Utilities.Log($"FMODRegistry failed to create sound \"{name}\" at {Utilities.GetCallingMethodName()}. Error: {result.ToString()}", LogLevel.Error);
        return false;
    }
    
    public static bool TryCreateSound(byte[] data, MODE mode, ref CREATESOUNDEXINFO extraInfo, out Sound sound)
    {
        RESULT result = System.createSound(data, mode, ref extraInfo, out sound);

        if (result == RESULT.OK)
        {
            return true;
        }

        Utilities.Log($"FMODRegistry failed to create sound \"MemoryStream\" at {Utilities.GetCallingMethodName()}. Error: {result.ToString()}", LogLevel.Error);
        return false;
    }
    
    public static bool TryPlaySound(Sound sound, ChannelGroup channelGroup, out Channel channel)
    {
        RESULT result = System.playSound(sound, channelGroup, false, out channel);

        if (result == RESULT.OK)
        {
            return true;
        }

        Utilities.Log($"FMODRegistry failed to play sound at {Utilities.GetCallingMethodName()}. Error: {result.ToString()}", LogLevel.Error);
        return false;
    }
    
    public static bool TryUpdate()
    {
        RESULT result = System.update();

        if (result == RESULT.OK)
        {
            return true;
        }

        Utilities.Log($"FMODRegistry failed to update core system at {Utilities.GetCallingMethodName()}. Error: {result.ToString()}", LogLevel.Error);
        return false;
    }

#endregion
}