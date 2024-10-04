using System;
using System.Runtime.CompilerServices;
using Babbler.Hooks;
using FMOD;
using BepInEx.Logging;
using Babbler.Implementation.Config;
using Babbler.Implementation.Occlusion;

namespace Babbler.Implementation.Common;

public static class FMODRegistry
{
    // All direct interactions with System are done through helper methods so we can log if there are errors.
    private static FMOD.System System;
    
    private static ChannelGroup NormalGroup;
    private static ChannelGroup PhoneGroup;

    public static void Initialize()
    {
        System = FMODUnity.RuntimeManager.CoreSystem;
        SetupGroups();
        Utilities.Log("FMODRegistry has initialized!", LogLevel.Debug);
    }
    
    private static void SetupGroups()
    {
        TryCreateChannelGroup("BabblerNormalGroup", out NormalGroup);
        TryCreateChannelGroup("BabblerPhoneGroup", out PhoneGroup);

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
    }

    public static ChannelGroup GetChannelGroup(SoundContext soundContext)
    {
        switch (soundContext)
        {
            case SoundContext.ConversationalSpeech:
            case SoundContext.ConversationalShout:
            case SoundContext.ConversationalEmote:
            case SoundContext.OverheardSpeech:
            case SoundContext.OverheardShout:
            case SoundContext.OverheardEmote:
                return NormalGroup;
            case SoundContext.PhoneSpeech:
            case SoundContext.PhoneShout:
            case SoundContext.PhoneEmote:
                return PhoneGroup;
        }
        
        return NormalGroup;
    }

    public static float GetVolume(SoundContext soundContext, OcclusionState occlusionState)
    {
        float babblerVolume = GetBaseVolume(soundContext) * GetOcclusionMultiplier(occlusionState);
        return VolumeCacheHook.MasterVolume * VolumeCacheHook.OtherVolume * babblerVolume;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float GetBaseVolume(SoundContext soundContext)
    {
        switch (soundContext)
        {
            case SoundContext.ConversationalSpeech:
                return BabblerConfig.ConversationalVolume.Value;
            case SoundContext.ConversationalShout:
                return BabblerConfig.ConversationalShoutVolume.Value;
            case SoundContext.ConversationalEmote:
                return BabblerConfig.ConversationalEmoteVolume.Value;
            case SoundContext.OverheardSpeech:
                return BabblerConfig.OverheardVolume.Value;
            case SoundContext.OverheardShout:
                return BabblerConfig.OverheardShoutVolume.Value;
            case SoundContext.OverheardEmote:
                return BabblerConfig.OverheardEmoteVolume.Value;
            case SoundContext.PhoneSpeech:
                return BabblerConfig.PhoneVolume.Value;
            case SoundContext.PhoneShout:
                return BabblerConfig.PhoneShoutVolume.Value;
            case SoundContext.PhoneEmote:
                return BabblerConfig.PhoneEmoteVolume.Value;
        }
        
        return BabblerConfig.ConversationalVolume.Value;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float GetOcclusionMultiplier(OcclusionState occlusionState)
    {
        switch (occlusionState)
        {
            case OcclusionState.NoOcclusion:
                return 1f;
            case OcclusionState.MuffleOpenDoor:
                return BabblerConfig.OpenDoorOcclusionMultiplier.Value;
            case OcclusionState.MuffleClosedDoor:
                return BabblerConfig.ClosedDoorOcclusionMultiplier.Value;
            case OcclusionState.MuffleVent:
                return BabblerConfig.VentOcclusionMultiplier.Value;
            case OcclusionState.DistantOcclusion:
                return BabblerConfig.DistantOcclusionMultiplier.Value;
            case OcclusionState.FullOcclusion:
                return 0f;
        }
        
        return 0f;
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