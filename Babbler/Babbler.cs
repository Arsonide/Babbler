using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FMOD;
using UnityEngine;

namespace Babbler;

public class Babbler : MonoBehaviour
{
    private const float PHONETIC_OVERLAP = 0.2f;
    private const float MIN_PITCH = 0.65f;
    private const float MAX_PITCH = 3f;
    
    public bool IsBabbling { get; private set; }
    
    private List<BabblePhonetic> _phoneticsToBabble = new List<BabblePhonetic>();
    private Coroutine _babbleCoroutine;
    
    private Human _currentHuman;
    private Transform _currentSourceTransform;
    private float _currentPitch;
    private float _currentVolume;
    private BabbleType _currentBabbleType;
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
        UpdateChannelPosition();
    }

    private void UpdateChannelPosition(bool force = false)
    {
        if (!IsBabbling || _currentSourceTransform == null)
        {
            return;
        }

        _t.position = _currentSourceTransform.position;

        if (!force && _lastPosition == _t.position)
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

    public void StartBabbleRoutine( string babbleInput, BabbleType babbleType, Human human)
    {
        StopBabbleRoutine();
        
        _currentBabbleType = babbleType;
        _currentHuman = human;

        _currentSourceTransform = babbleType != BabbleType.PhoneSpeech ? _currentHuman.lookAtThisTransform : Player.Instance?.phoneInteractable?.parentTransform;
        _currentPitch = Mathf.Lerp(MIN_PITCH, MAX_PITCH, 1f - _currentHuman.genderScale);
        
        switch (_currentBabbleType)
        {
            case BabbleType.FirstPersonSpeech:
            case BabbleType.PhoneSpeech:
                _currentVolume = BabblerPlugin.FirstPartyVolume;
                break;
            default:
                _currentVolume = BabblerPlugin.ThirdPartyVolume;
                break;
        }
        
        PopulatePhonetics(babbleInput);
        _babbleCoroutine = StartCoroutine(BabbleRoutine().WrapToIl2Cpp());
    }

    private IEnumerator BabbleRoutine()
    {
        IsBabbling = true;
        
        foreach (BabblePhonetic phonetic in _phoneticsToBabble)
        {
            FMODReferences.System.playSound(phonetic.Sound, FMODReferences.GetChannelGroup(_currentBabbleType), false, out _currentChannel);
            _currentChannel.setPitch(_currentPitch);
            _currentChannel.setVolume(_currentVolume);
            UpdateChannelPosition(true);
            
            yield return new WaitForSeconds(Mathf.Max(0f, phonetic.Length - PHONETIC_OVERLAP));
        }

        yield return null;
        
        // Releasing will set the gameobject inactive, which will StopBabbleRoutine.
        BabblerPool.ReleaseBabbler(this);
    }
}