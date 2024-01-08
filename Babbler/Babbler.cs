using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FMOD;
using UnityEngine;

namespace Babbler;

public class Babbler : MonoBehaviour
{
    public bool IsBabbling { get; private set; }
    
    private List<BabblePhonetic> _phoneticsToBabble = new List<BabblePhonetic>();
    private Coroutine _babbleCoroutine;
    
    private Human _currentHuman;
    private Transform _currentHumanTransform;
    private float _currentPitch;
    private Channel _currentChannel;

    private Transform _t;
    private Vector3 _lastPosition;
    
    private void Awake()
    {
        _t = transform;
    }
    
    private void OnEnable()
    {
        StopBabbleRoutine();
    }

    private void OnDisable()
    {
        StopBabbleRoutine();
    }

    private void Update()
    {
        if (!IsBabbling || _currentHumanTransform == null)
        {
            return;
        }

        _t.position = _currentHumanTransform.position;

        if (_lastPosition == _t.position)
        {
            return;
        }

        _lastPosition = _t.position;
                
        VECTOR position = new VECTOR
        {
            x = _lastPosition.x,
            y = _lastPosition.y,
            z = _lastPosition.z,
        };
                
        VECTOR velocity = new VECTOR
        {
            x = 0f,
            y = 0f,
            z = 0f,
        };

        _currentChannel.set3DAttributes(ref position, ref velocity);
        FMODUnity.RuntimeManager.CoreSystem.update();
    }

    private void PopulateHuman(Human human)
    {
        _currentHuman = human;
        _currentHumanTransform = _currentHuman.lookAtThisTransform;
        _currentPitch = Mathf.Lerp(0.5f, 2f, 1f - _currentHuman.genderScale);
    }
    
    private void PopulatePhonetics(string input)
    {
        _phoneticsToBabble.Clear();
        ReadOnlySpan<char> inputSpan = input.ToLowerInvariant().AsSpan();

        for (int i = 0; i < inputSpan.Length; ++i)
        {
            ReadOnlySpan<char> span = inputSpan.Slice(i, Math.Min(2, inputSpan.Length - i));
            string spanString = span.ToString();

            if (span.Length > 1 && BabblerPlugin.TryGetPhonetic(spanString, out BabblePhonetic phonetic))
            {
                _phoneticsToBabble.Add(phonetic);
                i++;
            }
            else if (BabblerPlugin.TryGetPhonetic(spanString[0].ToString(), out phonetic))
            {
                _phoneticsToBabble.Add(phonetic);
            }
        }
    }

    public void StopBabbleRoutine()
    {
        if (_babbleCoroutine != null)
        {
            StopCoroutine(_babbleCoroutine);
            _babbleCoroutine = null;
        }

        IsBabbling = false;
        _lastPosition = Vector3.zero;
    }

    public void StartBabbleRoutine(Human human, string babbleInput)
    {
        StopBabbleRoutine();
        PopulateHuman(human);
        PopulatePhonetics(babbleInput);
        _babbleCoroutine = StartCoroutine(BabbleRoutine().WrapToIl2Cpp());
    }

    private IEnumerator BabbleRoutine()
    {
        IsBabbling = true;
        
        foreach (BabblePhonetic phonetic in _phoneticsToBabble)
        {
            FMODUnity.RuntimeManager.CoreSystem.playSound(phonetic.Sound, BabblerPlugin.GetChannelGroup(), false, out _currentChannel);
            _currentChannel.setPitch(_currentPitch);
            FMODUnity.RuntimeManager.CoreSystem.update();

            while (_currentChannel.isPlaying(out bool isPlaying) == RESULT.OK && isPlaying)
            {
                yield return null;
            }
        }

        yield return null;
        
        // Releasing will set the gameobject inactive, which will StopBabbleRoutine.
        BabblerPool.ReleaseBabbler(this);
    }
}