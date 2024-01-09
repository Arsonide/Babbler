using HarmonyLib;

namespace Babbler.Patches;

[HarmonyPatch(typeof(SpeechBubbleController), "Setup")]
public class SpeechBubbleControllerPatch
{
    public static void Postfix(SpeechBubbleController __instance, SpeechController.QueueElement newSpeech, SpeechController newSpeechController)
    {
        string babbleInput = __instance?.actualString;

        if (string.IsNullOrWhiteSpace(babbleInput))
        {
            return;
        }

        // This is used for things like [Sneeze] or [Sigh]. Don't babble for emotes.
        if (babbleInput.StartsWith("[") && babbleInput.EndsWith("]"))
        {
            return;
        }

        // This should filter out things like "Zzz" and "Brrr".
        if (HasCharacterRepeated(babbleInput, 3))
        {
            return;
        }
        
        SpeechController controller = __instance?.speechController;
        Actor actor = controller?.actor;

        // Search around for a human because they don't seem to be assigned consistently?
        Human directHuman = actor as Human;
        Human aiHuman = actor?.ai?.human;

        Human speakingHuman = directHuman ?? aiHuman;
        Human telephoneHuman = GetOtherPhoneHuman(controller);
        
        Human anyHuman = speakingHuman ?? telephoneHuman;

        BabbleType babbleType;
        
        // We need to verify speaking human is null otherwise any time we are on a phone call ALL voices in the background are classified as phone voices.
        if (speakingHuman == null && telephoneHuman != null)
        {
            babbleType = BabbleType.PhoneSpeech;
        }
        else if (newSpeechController.actor != Player.Instance && InteractionController.Instance.dialogMode && InteractionController.Instance.talkingTo == newSpeechController.interactable)
        {
            babbleType = BabbleType.ConversationalSpeech;
        }
        else
        {
            babbleType = BabbleType.OverheardSpeech;
        }
        
        BabblerPool.Play(__instance.actualString, babbleType, anyHuman);
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

    private static Human GetOtherPhoneHuman(SpeechController controller)
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
}