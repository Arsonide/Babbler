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
        ConversationalGroup.setVolume(BabblerConfig.ConversationalVolume.Value);
        ConversationalShoutGroup.setVolume(BabblerConfig.ConversationalVolume.Value * BabblerConfig.ConversationalShoutMultiplier.Value);
    }

    private static void SetupOverheardGroup()
    {
        TryCreateChannelGroup("BabblerOverheardGroup", out OverheardGroup);
        TryCreateChannelGroup("BabblerOverheardShoutGroup", out OverheardShoutGroup);
        OverheardGroup.setVolume(BabblerConfig.OverheardVolume.Value);
        OverheardShoutGroup.setVolume(BabblerConfig.OverheardVolume.Value * BabblerConfig.OverheardShoutMultiplier.Value);
    }

    private static void SetupPhoneGroup()
    {
        TryCreateChannelGroup("BabblerPhoneGroup", out PhoneGroup);
        TryCreateChannelGroup("BabblerPhoneShoutGroup", out PhoneShoutGroup);
        PhoneGroup.setVolume(BabblerConfig.PhoneVolume.Value);
        PhoneShoutGroup.setVolume(BabblerConfig.PhoneVolume.Value * BabblerConfig.PhoneShoutMultiplier.Value);

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
        PhoneShoutGroup.addDSP(CHANNELCONTROL_DSP_INDEX.HEAD, phoneDSP);
    }

    public static ChannelGroup GetChannelGroup(SpeechContext speechContext)
    {
        switch (speechContext)
        {
            case SpeechContext.ConversationalSpeech:
                return ConversationalGroup;
            case SpeechContext.OverheardSpeech:
                return OverheardGroup;
            case SpeechContext.PhoneSpeech:
                return PhoneGroup;
            case SpeechContext.ConversationalShout:
                return ConversationalShoutGroup;
            case SpeechContext.OverheardShout:
                return OverheardShoutGroup;
            case SpeechContext.PhoneShout:
                return PhoneShoutGroup;
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