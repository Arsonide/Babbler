using BepInEx.Logging;
using HarmonyLib;
using Babbler.Implementation.Common;
using Babbler.Implementation.Hosts;
using UnityEngine;

namespace Babbler.Hooks;

[HarmonyPatch(typeof(SpeechBubbleController), "Setup")]
public class SpeechBubbleControllerHook
{
    [HarmonyPostfix]
    public static void Postfix(SpeechBubbleController __instance, SpeechController.QueueElement newSpeech, SpeechController newSpeechController)
    {
        string speechInput = __instance?.actualString;

        if (string.IsNullOrWhiteSpace(speechInput))
        {
            return;
        }

        // This is used for things like [Sneeze] or [Sigh]. Don't babble for emotes.
        if (speechInput.StartsWith("[") && speechInput.EndsWith("]"))
        {
            return;
        }

        // This should filter out things like "Zzz" and "Brrr".
        if (HasCharacterRepeated(speechInput, 3))
        {
            return;
        }
        
        SpeechController controller = __instance?.speechController;
        InteractableController interactable = controller?.interactable?.controller;
        Actor actor = controller?.actor;

        bool isPhoneBubble = controller?.phoneLine != null || (interactable != null && interactable.isPhone);

        // Search around for a human because they don't seem to be assigned consistently?
        Human directHuman = actor as Human;
        Human aiHuman = actor?.ai?.human;
        
        Human speakingHuman = directHuman ?? aiHuman;
        Human telephoneHuman = GetOtherPhoneHuman();
        
        // The operator is not a person that actually exists, so every 8 hours we pick a random citizen to serve as the operator.
        if (isPhoneBubble && telephoneHuman == null)
        {
            telephoneHuman = GetOperatorHuman();
        }
        
        Human anyHuman = speakingHuman ?? telephoneHuman;

        if (anyHuman == null)
        {
            Utilities.Log($"Babbler was unable to find a citizen for voice dialog: \"{__instance.actualString}\". Defaulting to a random citizen for now, but this should never happen!", LogLevel.Error);
            anyHuman = GetRandomHuman();
        }
        
        SpeechContext speechContext = GetSpeechContext(speechInput, speakingHuman, telephoneHuman, newSpeechController);
        SpeakerHostPool.Play(__instance.actualString, speechContext, anyHuman);
    }

    private static SpeechContext GetSpeechContext(string speechInput, Human speakingHuman, Human telephoneHuman, SpeechController speechController)
    {
        bool shouting = IsAllCaps(speechInput);
        
        // We need to verify speaking human is null otherwise any time we are on a phone call ALL voices in the background are classified as phone voices.
        if (speakingHuman == null && telephoneHuman != null)
        {
            return shouting ? SpeechContext.PhoneShout : SpeechContext.PhoneSpeech;
        }

        if (speechController.actor != Player.Instance && InteractionController.Instance.dialogMode && InteractionController.Instance.talkingTo == speechController.interactable)
        {
            return shouting ? SpeechContext.ConversationalShout : SpeechContext.ConversationalSpeech;
        }

        return shouting ? SpeechContext.OverheardShout : SpeechContext.OverheardSpeech;
    }
    
    private static bool HasCharacterRepeated(string input, int times)
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
                count++;
                
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
        int cityHash = CityData.Instance.seed.GetHashCode();

        // Operator shift changes once every 8 hours, or 28800 seconds.
        int currentOperatorShift = Mathf.RoundToInt(SessionData.Instance.gameTime / 28800f);
        
        // Now cantor pair those things to get a deterministic hash of them representing the current work shift.
        int sum = cityHash + currentOperatorShift;
        int cantor = (sum * (sum + 1) / 2) + currentOperatorShift;

        int citizenIndex = Mathf.Abs(cantor) % CityData.Instance.citizenDirectory.Count;
        Citizen citizen = CityData.Instance.citizenDirectory._items[citizenIndex];

        return citizen;
    }

    private static Human GetRandomHuman()
    {
        int citizenIndex = Utilities.GetRandomInteger(0, CityData.Instance.citizenDirectory.Count);
        Citizen citizen = CityData.Instance.citizenDirectory._items[citizenIndex];

        return citizen;
    }
}