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
    
    private List<PhoneticSound> _phoneticsToBabble = new List<PhoneticSound>();
    private List<Channel> _activeChannels = new List<Channel>();
    private Coroutine _babbleCoroutine;
    
    private Human _currentHuman;
    private Transform _currentSourceTransform;
    private float _currentPitch;
    private float _currentVolume;
    private BabbleType _currentBabbleType;

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
        Vector3 position = _currentSourceTransform.position;
        bool dirty = false;
        
        for (int i = _activeChannels.Count - 1; i >= 0; --i)
        {
            Channel channel = _activeChannels[i];
            
            if (channel.isPlaying(out bool isPlaying) == RESULT.OK && isPlaying)
            {
                SetChannelPosition(position, channel);
                dirty = true;
            }
            else
            {
                _activeChannels.RemoveAt(i);
            }
        }

        if (dirty)
        {
            FMODReferences.System.update();
        }
    }

    private void PopulatePhonetics(string input)
    {
        _phoneticsToBabble.Clear();
        ReadOnlySpan<char> inputSpan = input.ToLowerInvariant().AsSpan();

        for (int i = 0; i < inputSpan.Length; ++i)
        {
            ReadOnlySpan<char> span = inputSpan.Slice(i, Math.Min(2, inputSpan.Length - i));
            string spanString = span.ToString();

            if (span.Length > 1 && PhoneticSoundDatabase.TryGetPhonetic(spanString, out PhoneticSound phonetic))
            {
                _phoneticsToBabble.Add(phonetic);
                i++;
            }
            else if (PhoneticSoundDatabase.TryGetPhonetic(spanString[0].ToString(), out phonetic))
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
        _activeChannels.Clear();
    }

    public void StartBabbleRoutine( string babbleInput, BabbleType babbleType, Human human)
    {
        StopBabbleRoutine();
        
        _currentBabbleType = babbleType;
        _currentHuman = human;
        _currentSourceTransform = babbleType != BabbleType.PhoneSpeech ? _currentHuman.lookAtThisTransform : GetPlayerPhoneTransform();
        _currentPitch = Mathf.Lerp(BabblerConfig.MinimumPitch, BabblerConfig.MaximumPitch, 1f - _currentHuman.genderScale);
        
        switch (_currentBabbleType)
        {
            case BabbleType.ConversationalSpeech:
                _currentVolume = BabblerConfig.ConversationalVolume;
                break;
            case BabbleType.PhoneSpeech:
                _currentVolume = BabblerConfig.PhoneVolume;
                break;
            default:
                _currentVolume = BabblerConfig.OverheardVolume;
                break;
        }
        
        PopulatePhonetics(babbleInput);
        _babbleCoroutine = StartCoroutine(BabbleRoutine().WrapToIl2Cpp());
    }

    private IEnumerator BabbleRoutine()
    {
        IsBabbling = true;
        
        foreach (PhoneticSound phonetic in _phoneticsToBabble)
        {
            FMODReferences.System.playSound(phonetic.Sound, FMODReferences.GetChannelGroup(_currentBabbleType), false, out Channel channel);
            
            channel.setPitch(_currentPitch);
            channel.setVolume(_currentVolume);
        
            SetChannelPosition(_currentSourceTransform.position, channel);
            FMODReferences.System.update();

            _activeChannels.Add(channel);
            
            yield return new WaitForSeconds(Mathf.Max(0f, phonetic.Length - BabblerConfig.SyllableSpeed));
        }
        
        // Releasing will set the gameobject inactive, which will StopBabbleRoutine.
        BabblerPool.ReleaseBabbler(this);
    }

    private void SetChannelPosition(Vector3 position, Channel channel)
    {
        VECTOR pos = new VECTOR
        {
            x = position.x,
            y = position.y,
            z = position.z,
        };
                
        VECTOR vel = new VECTOR
        {
            x = 0f,
            y = 0f,
            z = 0f,
        };

        channel.set3DAttributes(ref pos, ref vel);
    }

    private Transform GetPlayerPhoneTransform()
    {
        // These all appear to be valid at different times so search for everything and fall back to the player GameObject if we can't find one.
        GameObject receiver = Player.Instance.interactingWith?.controller?.phoneReciever ??
                              Player.Instance.phoneInteractable?.controller?.phoneReciever ??
                              Player.Instance.answeringPhone?.interactable?.controller?.phoneReciever ??
                              Player.Instance.gameObject;

        return receiver?.transform;
    }
}