using Babbler.Implementation.Config;
using BepInEx.Logging;
using FMOD;

namespace Babbler.Implementation.Common;

public static class FMODRegistry
{
    // All direct interactions with System are done through helper methods so we can log if there are errors.
    private static FMOD.System System;
    
    private static ChannelGroup SpeechGroup;
    private static ChannelGroup PhoneGroup;
    public static ChannelGroup ShoutGroup;
    
    public static void Initialize()
    {
        // TODO: Set SpeechContext volume here on the ChannelGroups so we don't need to do it every time we play a sound.
        System = FMODUnity.RuntimeManager.CoreSystem;
        TryCreateChannelGroup("BabblerSpeechGroup", out SpeechGroup);
        SetupPhoneEffects();
        SetupShoutEffects();
    }

    public static ChannelGroup GetChannelGroup(SpeechContext speechContext)
    {
        return speechContext == SpeechContext.PhoneSpeech && BabblerConfig.DistortPhoneSpeech ? PhoneGroup : SpeechGroup;
    }

    private static void SetupPhoneEffects()
    {
        TryCreateChannelGroup("BabblerPhoneGroup", out PhoneGroup);

        TryCreateDSP(DSP_TYPE.MULTIBAND_EQ, out DSP phoneDSP);

        phoneDSP.setParameterInt((int)DSP_MULTIBAND_EQ.A_FILTER, (int)DSP_MULTIBAND_EQ_FILTER_TYPE.HIGHPASS_48DB);
        phoneDSP.setParameterFloat((int)DSP_MULTIBAND_EQ.A_FREQUENCY, 300f);
        
        phoneDSP.setParameterInt((int)DSP_MULTIBAND_EQ.B_FILTER, (int)DSP_MULTIBAND_EQ_FILTER_TYPE.PEAKING);
        phoneDSP.setParameterFloat((int)DSP_MULTIBAND_EQ.B_FREQUENCY, 1700f);
        phoneDSP.setParameterFloat((int)DSP_MULTIBAND_EQ.B_Q, 1f);

        phoneDSP.setParameterInt((int)DSP_MULTIBAND_EQ.C_FILTER, (int)DSP_MULTIBAND_EQ_FILTER_TYPE.LOWPASS_48DB);
        phoneDSP.setParameterFloat((int)DSP_MULTIBAND_EQ.C_FREQUENCY, 3400f);
        
        PhoneGroup.addDSP(CHANNELCONTROL_DSP_INDEX.HEAD, phoneDSP);
    }

    private static void SetupShoutEffects()
    {
        // Not used currently, just something I'm experimenting with for ALL CAPS DIALOG THAT IS DETECTED.
        TryCreateChannelGroup("BabblerShoutGroup", out ShoutGroup);
        
        TryCreateDSP(DSP_TYPE.DISTORTION, out DSP distortionDSP);
        distortionDSP.setParameterFloat((int)DSP_DISTORTION.LEVEL, 0.5f);
        
        TryCreateDSP(DSP_TYPE.MULTIBAND_EQ, out DSP eqDSP);
        eqDSP.setParameterFloat((int)DSP_MULTIBAND_EQ.A_GAIN, 2.0f);
        eqDSP.setParameterFloat((int)DSP_MULTIBAND_EQ.A_FREQUENCY, 2500f);
        
        TryCreateDSP(DSP_TYPE.TREMOLO, out DSP tremoloDSP);
        tremoloDSP.setParameterFloat((int)DSP_TREMOLO.FREQUENCY, 6f);
        tremoloDSP.setParameterFloat((int)DSP_TREMOLO.DEPTH, 0.4f);
        
        TryCreateDSP(DSP_TYPE.COMPRESSOR, out DSP compressorDSP);
        compressorDSP.setParameterFloat((int)DSP_COMPRESSOR.THRESHOLD, -10f);
        compressorDSP.setParameterFloat((int)DSP_COMPRESSOR.RATIO, 4f);
        
        ShoutGroup.addDSP(CHANNELCONTROL_DSP_INDEX.HEAD, distortionDSP);
        ShoutGroup.addDSP(CHANNELCONTROL_DSP_INDEX.HEAD, eqDSP);
        ShoutGroup.addDSP(CHANNELCONTROL_DSP_INDEX.HEAD, tremoloDSP);
        ShoutGroup.addDSP(CHANNELCONTROL_DSP_INDEX.HEAD, compressorDSP);
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