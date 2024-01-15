using Babbler.Implementation.Common;

namespace Babbler.Implementation.Speakers;

public class DroningSpeaker : PhoneticSpeaker
{
    protected override void ProcessSpeechInput(Human speechPerson, ref string speechInput)
    {
        base.ProcessSpeechInput(speechPerson, ref speechInput);
        
        char monosyllable = PickMonosyllable(speechPerson);
        Utilities.GlobalStringBuilder.Clear();
        
        foreach (char c in speechInput)
        {
            Utilities.GlobalStringBuilder.Append(char.IsLetter(c) ? monosyllable : c);
        }

        speechInput = Utilities.GlobalStringBuilder.ToString();
    }
}