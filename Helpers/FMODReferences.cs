using FMOD;

namespace Babbler;

public static class FMODReferences
{
    public static FMOD.System System { get; private set; }
    
    private static ChannelGroup SpeechGroup;
    private static ChannelGroup PhoneGroup;

    private static DSP PhoneDSP;
    
    public static void Initialize()
    {
        System = FMODUnity.RuntimeManager.CoreSystem;
        
        System.createChannelGroup("BabblerSpeechGroup", out SpeechGroup);
        System.createChannelGroup("BabblerPhoneGroup", out PhoneGroup);
        
        System.createDSPByType(DSP_TYPE.MULTIBAND_EQ, out PhoneDSP);

        PhoneDSP.setParameterInt((int)DSP_MULTIBAND_EQ.A_FILTER, (int)DSP_MULTIBAND_EQ_FILTER_TYPE.HIGHPASS_48DB);
        PhoneDSP.setParameterFloat((int)DSP_MULTIBAND_EQ.A_FREQUENCY, 300f);
        
        PhoneDSP.setParameterInt((int)DSP_MULTIBAND_EQ.B_FILTER, (int)DSP_MULTIBAND_EQ_FILTER_TYPE.PEAKING);
        PhoneDSP.setParameterFloat((int)DSP_MULTIBAND_EQ.B_FREQUENCY, 1700f);
        PhoneDSP.setParameterFloat((int)DSP_MULTIBAND_EQ.B_Q, 1f);

        PhoneDSP.setParameterInt((int)DSP_MULTIBAND_EQ.C_FILTER, (int)DSP_MULTIBAND_EQ_FILTER_TYPE.LOWPASS_48DB);
        PhoneDSP.setParameterFloat((int)DSP_MULTIBAND_EQ.C_FREQUENCY, 3400f);

        PhoneGroup.addDSP(CHANNELCONTROL_DSP_INDEX.HEAD, PhoneDSP);
    }

    public static ChannelGroup GetChannelGroup(BabbleType babbleType)
    {
        return babbleType == BabbleType.PhoneSpeech && BabblerConfig.DistortPhoneSpeech ? PhoneGroup : SpeechGroup;
    }
}