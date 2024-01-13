namespace Babbler.Implementation.Blurbs;

public class BlurbSound
{
    // TODO make a "blurb sound list" so we can have multiple blurb voice profiles, like synthesis.
    public string Phonetic;
    public string FilePath;
    public FMOD.Sound Sound;
    public float Length;
    public bool Released;
}