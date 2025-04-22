using BepInEx.Logging;
using HarmonyLib;
using Babbler.Implementation.Common;
using Babbler.Implementation.Config;
using Babbler.Implementation.Emotes;
using Babbler.Implementation.Hosts;
using UnityEngine;
using System.Collections;
using BepInEx;
using System.Linq;
using System.Text.RegularExpressions;

namespace Babbler.Hooks;

[HarmonyPatch(typeof(SpeechBubbleController), "Setup")]
public class SpeechBubbleControllerHook
{
    private static Human LastPhoneHuman;
    
    [HarmonyPostfix]
    public static void Postfix(SpeechBubbleController __instance, SpeechController.QueueElement newSpeech, SpeechController newSpeechController)
    {
        string speechInput = __instance?.actualString;

        if (string.IsNullOrWhiteSpace(speechInput))
        {
            return;
        }

        // Ignore player dialogue, thoughts, and cutscene messages.
        if (__instance.isPlayer || CutSceneController.Instance.cutSceneActive)
        {
            return;
        }

        // Before we process the line, check if we're replacing it.
        if (ReplacementRegistry.TryGetReplacement(speechInput, out string replacement))
        {
            speechInput = replacement;
        }
        
        // This should filter out things like "Zzz" and "Brrr".
        if (HasCharacterRepeated(speechInput, 3, true))
        {
            return;
        }

        BubbleHumanSearch(__instance, out Human speakingHuman, out Human telephoneHuman, out Human anyHuman);

        // Something bad happened, just early out.
        if (anyHuman == null)
        {
            Utilities.Log($"Babbler unable to find a human to associate with message \"{speechInput}\"!", LogLevel.Debug);
            return;
        }

        // This is used for things like [Sneeze] or [Sigh].
        bool emoteSpeech = IsEmoteSpeech(speechInput);

        if (emoteSpeech && !BabblerConfig.EmotesEnabled.Value)
        {
            return;
        }
        
        SoundContext soundContext = GetSoundContext(speechInput, emoteSpeech, speakingHuman, telephoneHuman, newSpeechController);

        // When shouting we raise the volume, but TTS interprets all caps as acronyms a lot, lowering the case makes it more natural sounding.
        switch (soundContext)
        {
            case SoundContext.ConversationalShout:
            case SoundContext.OverheardShout:
            case SoundContext.PhoneShout:
                speechInput = speechInput.ToLowerInvariant();
                break;
        }

        if (emoteSpeech)
        {
            speechInput = StripEmoteSpeech(speechInput).ToLowerInvariant();

            if (EmoteSoundRegistry.HasEmote(speechInput))
            {
                SpeakerHostPool.Emotes.Play(speechInput, soundContext, anyHuman);
            }
        }
        else
        {
            UniverseLib.RuntimeHelper.StartCoroutine(SpeechLoop(__instance, soundContext, anyHuman));
        }
    }

    /// <summary>
    /// This method assumes that content is being streamed into the speech bubble in real-time and checks every n seconds for new content.
    /// </summary>
    /// <param name="speechBubble"></param>
    /// <param name="soundContext"></param>
    /// <param name="anyHuman"></param>
    /// <param name="checkFrequency"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static IEnumerator SpeechLoop(SpeechBubbleController speechBubble, SoundContext soundContext, Human anyHuman, float timeout = 5f)
    {
        // These variables persist between checks
        SpeakerHost currentSpeaker = null;
        int lastSpokenIndex = -1;
        char[] sentenceDelimiters = { '.', ';', '?', '!', "\n"[0] };
        char nonverbalDelimiter = '*'; // for denoting actions rather than spoken dialogue, since LLMs love to throw these in
        float timeSpentWaitingToSpeak = 0, checkFrequency = 0.1f;
        bool narrateNonverbal = BabblerConfig.NarrateActions.Value;


        while (timeSpentWaitingToSpeak < timeout)
        {
            yield return new WaitForSeconds(checkFrequency);
            if (currentSpeaker != null && currentSpeaker.Speaker.isSpeaking)
                continue;
            timeSpentWaitingToSpeak += checkFrequency;

            // if the speech bubble is empty or is placeholder text, then we're waiting for streaming to start
            if (speechBubble.actualString.Length == 0
                || (speechBubble.actualString.Length > 0 && speechBubble.actualString.Replace(" . ","").IsNullOrWhiteSpace()))
            {
                continue;
            }

            string textToParse =  speechBubble.actualString;
            string speechInput = "";
            bool sentenceCompleted = false;
            int nonverbalCount = 0;
            int sentenceStart = 0, sentenceEnd = -1;

            //Step 1: replace rich text tags (e.g. <color=#ffffff>) -- SmartCitizens will try to format nonverbal actions this way
            var rgx = new Regex("<.+?>");
            textToParse = rgx.Replace(textToParse, nonverbalDelimiter.ToString());
            //condense tags
            rgx = new Regex($"\\{nonverbalDelimiter}+");
            textToParse = rgx.Replace(textToParse, nonverbalDelimiter.ToString());

            //Step 2: iterate over each character in the remaining text until we hit a sentence delimiter
            for (int index = 0; index < textToParse.Length; index++)
            {
                char c = textToParse[index];

                if (nonverbalCount % 2 == 0                // i.e. outside nonverbal section
                    && (sentenceDelimiters.Contains(c))   // we've hit a sentence ender
                    && index > lastSpokenIndex)          // this prevents repeating the same sentences
                {
                    sentenceCompleted = true;
                    sentenceEnd = index;
                    speechInput = textToParse.Substring(sentenceStart, sentenceEnd - sentenceStart + 1);
                    // Debug.Log($"To speak:{speechInput}");
                    break;
                }
                // restart sentence if...
                else if (!narrateNonverbal && c == nonverbalDelimiter) // ...beginning of nonverbal section
                {
                    nonverbalCount++;
                    sentenceStart = index+1;
                }
                else if (nonverbalCount % 2 == 1) // ...inside nonverbal section
                {
                    sentenceStart = index+1;
                }
                else if(sentenceDelimiters.Contains(c)) // ...this is a sentence delimiter, but the sentence didn't qualify
                {
                    sentenceStart = index+1;
                }
            }

            //Step 3: Speak the new sentence
            if (sentenceCompleted)
            {
                lastSpokenIndex = sentenceEnd;
                if (!speechInput.IsNullOrWhiteSpace())
                {
                    speechInput = speechInput.Replace(nonverbalDelimiter.ToString(), ""); // avoids saying 'asterisk' when narrating
                    currentSpeaker = SpeakerHostPool.Speech.Play(speechInput, soundContext, anyHuman);
                    timeSpentWaitingToSpeak = 0;
                }
            }
            
        }
        
    }

    private static SoundContext GetSoundContext(string speechInput, bool isEmote, Human speakingHuman, Human telephoneHuman, SpeechController speechController)
    {
        bool shouting = IsAllCaps(speechInput);
        
        // We need to verify speaking human is null otherwise any time we are on a phone call ALL voices in the background are classified as phone voices.
        if (speakingHuman == null && telephoneHuman != null)
        {
            if (isEmote)
            {
                return SoundContext.PhoneEmote;
            }

            if (shouting)
            {
                return SoundContext.PhoneShout;
            }

            return SoundContext.PhoneSpeech;
        }

        if (speechController.actor != Player.Instance && InteractionController.Instance.dialogMode && InteractionController.Instance.talkingTo == speechController.interactable)
        {
            if (isEmote)
            {
                return SoundContext.ConversationalEmote;
            }

            if (shouting)
            {
                return SoundContext.ConversationalShout;
            }

            return SoundContext.ConversationalSpeech;
        }

        if (isEmote)
        {
            return SoundContext.OverheardEmote;
        }

        if (shouting)
        {
            return SoundContext.OverheardShout;
        }

        return SoundContext.OverheardSpeech;
    }

    private static bool IsEmoteSpeech(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return true;
        }
        
        switch (input[0])
        {
            case '[':
                return input.EndsWith("]");
            case '(':
                return input.EndsWith(")");
            case '{':
                return input.EndsWith("}");
            case '<':
                return input.EndsWith(">");
            default:
                return false;
        }
    }
    
    private static string StripEmoteSpeech(string speechInput)
    {
        // First and last characters will be braces or brackets of some kind, so substring it.
        if (string.IsNullOrEmpty(speechInput) || speechInput.Length <= 2)
        {
            return string.Empty;
        }
        
        return speechInput.Substring(1, speechInput.Length - 2);
    }
    
    private static bool HasCharacterRepeated(string input, int times, bool lettersOnly)
    {
        if (string.IsNullOrEmpty(input) || times <= 0)
        {
            return false;
        }

        string inputLower = input.ToLowerInvariant();
        int count = 1;
        
        for (int i = 1; i < inputLower.Length; i++)
        {
            if (inputLower[i] == inputLower[i - 1])
            {
                if (!lettersOnly || char.IsLetter(inputLower[i]))
                {
                    count++;
                }

                if (count >= times)
                {
                    return true;
                }
            }
            else
            {
                count = 1;
            }
        }

        return false;
    }
    
    private static bool IsAllCaps(string input)
    {
        foreach (char c in input)
        {
            if (char.IsLetter(c) && !char.IsUpper(c))
            {
                return false;
            }
        }
        
        return true;
    }

#region Human Helpers

    private static void BubbleHumanSearch(SpeechBubbleController bubbleController, out Human speakingHuman, out Human telephoneHuman, out Human anyHuman)
    {
        SpeechController controller = bubbleController?.speechController;
        InteractableController interactable = controller?.interactable?.controller;
        Actor actor = controller?.actor;
        
        bool isPhoneBubble = controller?.phoneLine != null || (interactable != null && interactable.isPhone);

        // Search around for a human because they don't seem to be assigned consistently?
        Human directHuman = actor as Human;
        Human aiHuman = actor?.ai?.human;
        
        speakingHuman = directHuman ?? aiHuman;
        telephoneHuman = GetOtherPhoneHuman();
       
        if (isPhoneBubble)
        {
            // This can happen in two instances, talking to an operator during an active call, or talking to a fake human (a mission giver) and then releasing the phone, so you are nearby but not holding it.
            if (telephoneHuman == null)
            {
                TelephoneController.PhoneCall call = Player.Instance?.activeCall;

                if (call != null)
                {
                    // If we're on a call, then it's the operator. The operator is not a person that actually exists, so every 8 hours we pick a random citizen to serve as the operator.
                    telephoneHuman = GetOperatorHuman();
                    LastPhoneHuman = telephoneHuman;
                }
                else
                {
                    // If we're not on a call, then we released the phone (so we no longer have access to references we need to get the person on the other end. We use a cache in this instance.
                    telephoneHuman = LastPhoneHuman;
                }
            }
            else
            {
                LastPhoneHuman = telephoneHuman;
            }
        }
        
        anyHuman = speakingHuman ?? telephoneHuman;
    }
    
    private static Human GetOtherPhoneHuman()
    {
        TelephoneController.PhoneCall call = Player.Instance?.activeCall;

        if (call == null)
        {
            return null;
        }
        
        if (call.source.job > -1 && SideJobController.Instance.allJobsDictionary.TryGetValue(call.source.job, out SideJob job))
        {
            return job.poster;
        }
        
        if (CityData.Instance.GetHuman(call.caller, out Human caller) && caller != null && !caller.isPlayer)
        {
            return caller;
        }

        if (CityData.Instance.GetHuman(call.receiver, out Human receiver) && receiver != null && !receiver.isPlayer)
        {
            return receiver;
        }

        return null;
    }

    private static Human GetOperatorHuman()
    {
        int cityHash = Utilities.GetDeterministicStringHash(CityData.Instance.seed);

        // Operator shift changes once every 8 hours, and gameTime is in hours.
        int currentOperatorShift = Mathf.RoundToInt(SessionData.Instance.gameTime / 8f);
        
        // Now cantor pair those things to get a deterministic hash of them representing the current work shift.
        int sum = cityHash + currentOperatorShift;
        int cantor = (sum * (sum + 1) / 2) + currentOperatorShift;

        int citizenIndex = Mathf.Abs(cantor) % CityData.Instance.citizenDirectory.Count;
        Citizen citizen = CityData.Instance.citizenDirectory._items[citizenIndex];

        return citizen;
    }

#endregion
}
