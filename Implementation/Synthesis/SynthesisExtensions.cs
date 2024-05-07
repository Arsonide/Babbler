#pragma warning disable CA1416

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Speech.Synthesis;

namespace Babbler.Implementation.Synthesis;

public static class SynthesisExtensions
{
    public static List<InstalledVoice> GetOneCoreVoices(this SpeechSynthesizer synthesizer)
    {
        List<InstalledVoice> results = new List<InstalledVoice>();
        Type type = typeof(SpeechSynthesizer);
        object voiceSynthesizer = type.GetProperty("VoiceSynthesizer", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(synthesizer);
        
        if (voiceSynthesizer == null)
        {
            return results;
        }

        Assembly assembly = type.Assembly;
        Type tokenCategoryType = assembly.GetType("System.Speech.Internal.ObjectTokens.ObjectTokenCategory");

        if (tokenCategoryType == null)
        {
            return results;
        }
        
        Type voiceInfoType = typeof(VoiceInfo);
        Type installedVoiceType = typeof(InstalledVoice);
        
        string voiceInfoName = voiceInfoType.FullName;
        string installedVoiceName = installedVoiceType.FullName;
        
        if (string.IsNullOrEmpty(voiceInfoName) || string.IsNullOrEmpty(installedVoiceName))
        {
            return results;
        }
        
        if (tokenCategoryType.GetMethod("Create", BindingFlags.Static | BindingFlags.NonPublic)?.Invoke(null, new object[] { @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech_OneCore\Voices" }) is not IDisposable categoryType)
        {
            return results;
        }

        using (categoryType)
        {
            if (tokenCategoryType.GetMethod("FindMatchingTokens", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(categoryType, new object[] { null, null }) is not IList tokenList)
            {
                return results;
            }

            foreach (object token in tokenList)
            {
                if (token == null)
                {
                    continue;
                }

                if (token.GetType().GetProperty("Attributes", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(token) == null)
                {
                    continue;
                }

                object voiceInfo = assembly.CreateInstance(voiceInfoName, true, BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { token }, null, null);

                if (voiceInfo == null)
                {
                    continue;
                }

                object installedObject = assembly.CreateInstance(installedVoiceName, true, BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { voiceSynthesizer, voiceInfo }, null, null);
                
                if (installedObject is not InstalledVoice installedVoice)
                {
                    continue;
                }

                results.Add(installedVoice);
            }
        }

        return results;
    }

    public static void AddVoices(this SpeechSynthesizer synthesizer, List<InstalledVoice> voices)
    {
        Type type = typeof(SpeechSynthesizer);
        object voiceSynthesizer = type.GetProperty("VoiceSynthesizer", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(synthesizer);

        if (voiceSynthesizer == null)
        {
            return;
        }

        IList installedVoices = type.GetField("_installedVoices", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(voiceSynthesizer) as IList;

        if (installedVoices == null)
        {
            return;
        }

        foreach (InstalledVoice installedVoice in voices)
        {
            installedVoices.Add(installedVoice);
        }
    }
}

#pragma warning restore CA1416