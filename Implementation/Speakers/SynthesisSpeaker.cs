#pragma warning disable CA1416

using System.IO;
using System.Runtime.InteropServices;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using UnityEngine;
using FMOD;
using Babbler.Implementation.Characteristics;
using Babbler.Implementation.Common;
using Babbler.Implementation.Config;
using Babbler.Implementation.Synthesis;

namespace Babbler.Implementation.Speakers;

public class SynthesisSpeaker : BaseSpeaker
{
    private SpeechSynthesizer _synthesizer;
    private MemoryStream _memoryStream;
    
    // TODO Figure out the lifetime of SpeakerHost, these may be getting destroyed on scene load for no reason, and that might mess up the pool.
    public override void InitializeSpeaker()
    {
        base.InitializeSpeaker();

        _synthesizer = new SpeechSynthesizer();
        _memoryStream = new MemoryStream();
        
        _synthesizer.SetOutputToAudioStream(_memoryStream, new SpeechAudioFormatInfo(44100, AudioBitsPerSample.Sixteen, AudioChannel.Mono));
    }

    public override void UninitializeSpeaker()
    {
        base.UninitializeSpeaker();
        
        _synthesizer.Dispose();
        _memoryStream.Dispose();
    }

    public override void StopSpeaker()
    {
        base.StopSpeaker();

        _synthesizer.SpeakCompleted -= OnSpeakCompleted;
        _synthesizer.SpeakAsyncCancelAll();
    }

    public override void StartSpeaker(string speechInput, SpeechContext speechContext, Human speechPerson)
    {
        base.StartSpeaker(speechInput, speechContext, speechPerson);

        _synthesizer.SpeakCompleted -= OnSpeakCompleted;
        _synthesizer.SpeakCompleted += OnSpeakCompleted;
        
        _synthesizer.SpeakAsync(speechInput);
    }

    private void OnSpeakCompleted(object sender, SpeakCompletedEventArgs args)
    {
        if (!(sender is SpeechSynthesizer synthesizer))
        {
            return;
        }
        
        synthesizer.SpeakCompleted -= OnSpeakCompleted;

        _memoryStream.Position = 0;

        CREATESOUNDEXINFO soundInfo = new CREATESOUNDEXINFO
        {
            cbsize = Marshal.SizeOf(typeof(CREATESOUNDEXINFO)),
            length = (uint)_memoryStream.Length,
            format = SOUND_FORMAT.PCM16,
            defaultfrequency = 44100,
            numchannels = 1,
        };

        if (!FMODRegistry.TryCreateSound(_memoryStream.ToArray(), MODE.OPENMEMORY | MODE.OPENRAW | MODE._3D, ref soundInfo, out Sound sound))
        {
            return;
        }

        if (!FMODRegistry.TryPlaySound(sound, FMODRegistry.GetChannelGroup(SpeechContext), out Channel channel))
        {
            return;
        }
        
        channel.setPitch(SpeechPitch);
        channel.setVolume(SpeechVolume);
        
        SetChannelPosition(SpeechSource.position, channel);
        FMODRegistry.TryUpdate();
    }
    
    protected override float CacheSpeechPitch(Human speechPerson)
    {
        // While we have a human, use this as an opportunity to choose the SpeechSynthesis voice.
        string voice = SynthesisVoiceRegistry.GetVoice(speechPerson, out VoiceCharacteristics characteristics);
        _synthesizer.SelectVoice(voice);

        switch (characteristics.Category)
        {
            case VoiceCategory.Male:
                return Mathf.Lerp(BabblerConfig.SynthesisPitchMaleMinimum, BabblerConfig.SynthesisPitchMaleMaximum, characteristics.Pitch);
            case VoiceCategory.Female:
                return Mathf.Lerp(BabblerConfig.SynthesisPitchFemaleMinimum, BabblerConfig.SynthesisPitchFemaleMaximum, characteristics.Pitch);
            default:
                return Mathf.Lerp(BabblerConfig.SynthesisPitchNonBinaryMinimum, BabblerConfig.SynthesisPitchNonBinaryMaximum, characteristics.Pitch);
        }
    }
}

#pragma warning restore CA1416