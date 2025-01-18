#pragma warning disable CA1416

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using UnityEngine;
using FMOD;
using Babbler.Implementation.Characteristics;
using Babbler.Implementation.Common;
using Babbler.Implementation.Config;
using Babbler.Implementation.Occlusion;
using Babbler.Implementation.Synthesis;
using BepInEx.Logging;

namespace Babbler.Implementation.Speakers;

public class SynthesisSpeaker : BaseSpeaker
{
    private SpeechSynthesizer _synthesizer;
    private MemoryStream _memoryStream;
    
    public override void InitializeSpeaker()
    {
        base.InitializeSpeaker();

        _synthesizer = new SpeechSynthesizer();

        try
        {
            if (SynthesisVoiceRegistry.OneCoreVoices != null)
            {
                _synthesizer.AddVoices(SynthesisVoiceRegistry.OneCoreVoices);
            }
        }
        catch (Exception e)
        {
            Utilities.Log($"Exception encountered while SynthesisSpeaker tried to add OneCore voices: {e.Message}", LogLevel.Debug);
        }

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

    public override void StartSpeaker(string speechInput, SoundContext soundContext, Human speechPerson)
    {
        if (BabblerConfig.SynthesisDebugVoiceName.Value)
        {
            speechInput = _synthesizer.Voice.Name + ": " + speechInput;
        }

        base.StartSpeaker(speechInput, soundContext, speechPerson);

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

        if (!FMODRegistry.TryPlaySound(sound, FMODRegistry.GetChannelGroup(SoundContext), out Channel channel))
        {
            return;
        }
        
        channel.setPitch(SpeechPitch);
        SetChannelPosition(SpeechSource.position, channel);
        SetChannelVolume(OcclusionState.NoOcclusion, channel);
        FMODRegistry.TryUpdate();
        
        ActiveChannels.Add(channel);
    }
    
    protected override float CacheSpeechPitch(Human speechPerson, string speechInput)
    {
        // While we have a human, use this as an opportunity to choose the SpeechSynthesis voice.
        string voice = SynthesisVoiceRegistry.GetVoice(speechPerson, out VoiceCharacteristics characteristics);
        _synthesizer.SelectVoice(voice);
        _synthesizer.Rate = Mathf.RoundToInt(Mathf.Lerp(BabblerConfig.SynthesisMinSpeed.Value, BabblerConfig.SynthesisMaxSpeed.Value, characteristics.Rate));

        switch (characteristics.Category)
        {
            case VoiceCategory.Male:
                return Mathf.Lerp(BabblerConfig.SynthesisMinPitchMale.Value, BabblerConfig.SynthesisMaxPitchMale.Value, characteristics.Pitch);
            case VoiceCategory.Female:
                return Mathf.Lerp(BabblerConfig.SynthesisMinPitchFemale.Value, BabblerConfig.SynthesisMaxPitchFemale.Value, characteristics.Pitch);
            case VoiceCategory.NonBinary:
            default:
                return Mathf.Lerp(BabblerConfig.SynthesisMinPitchNonBinary.Value, BabblerConfig.SynthesisMaxPitchNonBinary.Value, characteristics.Pitch);
        }
    }

    protected override void OnLastChannelFinished()
    {
        base.OnLastChannelFinished();
        _memoryStream.Seek(0, SeekOrigin.Begin);
        _memoryStream.SetLength(0);
        OnFinishedSpeaking?.Invoke();
    }
}

#pragma warning restore CA1416