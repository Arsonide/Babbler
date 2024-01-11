using FMOD;

namespace Babbler;

public static class FMODReferences
{
    public static FMOD.System System { get; private set; }
    
    private static ChannelGroup SpeechGroup;
    private static ChannelGroup PhoneGroup;
    public static ChannelGroup ShoutGroup;
    
    public static void Initialize()
    {
        // TODO: Set SpeechContext volume here on the ChannelGroups so we don't need to do it every time we play a sound.
        System = FMODUnity.RuntimeManager.CoreSystem;
        System.createChannelGroup("BabblerSpeechGroup", out SpeechGroup);
        SetupPhoneEffects();
        SetupShoutEffects();
    }

    public static ChannelGroup GetChannelGroup(SpeechContext speechContext)
    {
        return speechContext == SpeechContext.PhoneSpeech && BabblerConfig.DistortPhoneSpeech ? PhoneGroup : SpeechGroup;
    }

    private static void SetupPhoneEffects()
    {
        System.createChannelGroup("BabblerPhoneGroup", out PhoneGroup);

        System.createDSPByType(DSP_TYPE.MULTIBAND_EQ, out DSP phoneDSP);

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
        System.createChannelGroup("BabblerShoutGroup", out ShoutGroup);
        
        System.createDSPByType(DSP_TYPE.DISTORTION, out DSP distortionDsp);
        distortionDsp.setParameterFloat((int)DSP_DISTORTION.LEVEL, 0.5f);
        
        System.createDSPByType(DSP_TYPE.MULTIBAND_EQ, out DSP eqDsp);
        eqDsp.setParameterFloat((int)DSP_MULTIBAND_EQ.A_GAIN, 2.0f);
        eqDsp.setParameterFloat((int)DSP_MULTIBAND_EQ.A_FREQUENCY, 2500f);
        
        System.createDSPByType(DSP_TYPE.TREMOLO, out DSP tremoloDsp);
        tremoloDsp.setParameterFloat((int)DSP_TREMOLO.FREQUENCY, 6f);
        tremoloDsp.setParameterFloat((int)DSP_TREMOLO.DEPTH, 0.4f);
        
        System.createDSPByType(DSP_TYPE.COMPRESSOR, out DSP compressorDsp);
        compressorDsp.setParameterFloat((int)DSP_COMPRESSOR.THRESHOLD, -10f);
        compressorDsp.setParameterFloat((int)DSP_COMPRESSOR.RATIO, 4f);
        
        ShoutGroup.addDSP(CHANNELCONTROL_DSP_INDEX.HEAD, distortionDsp);
        ShoutGroup.addDSP(CHANNELCONTROL_DSP_INDEX.HEAD, eqDsp);
        ShoutGroup.addDSP(CHANNELCONTROL_DSP_INDEX.HEAD, tremoloDsp);
        ShoutGroup.addDSP(CHANNELCONTROL_DSP_INDEX.HEAD, compressorDsp);
    }
}