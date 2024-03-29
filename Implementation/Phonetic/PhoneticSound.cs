﻿using UnityEngine;

namespace Babbler.Implementation.Phonetic;

public class PhoneticSound
{
    public string Phonetic;
    public string FilePath;
    public FMOD.Sound Sound;
    public float Length;
    public bool Released;
}