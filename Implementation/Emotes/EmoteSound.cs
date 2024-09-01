using Babbler.Implementation.Characteristics;

namespace Babbler.Implementation.Emotes;

public class EmoteSound
{
    public string Key;
    public string FilePath;
    public VoiceCategory Category;
    public float Frequency;
    public bool CanPitchShift;
    public FMOD.Sound Sound;
    public float Length;
    public bool Released;
}