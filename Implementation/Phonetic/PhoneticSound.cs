using UnityEngine;

namespace Babbler.Implementation.Phonetic;

public class PhoneticSound
{
    // TODO make a "phonetic voice" so we can have multiple phonetic voice profiles, like synthesis.
    public string Phonetic;
    public string FilePath;
    public FMOD.Sound Sound;
    public float Length;
    public bool Released;
}