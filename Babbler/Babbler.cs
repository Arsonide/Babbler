using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using UnityEngine;
using FMOD;

namespace Babbler;

public class Babbler : MonoBehaviour
{
    private readonly List<PhoneticSound> _phoneticsToBabble = new List<PhoneticSound>();
    private readonly List<Channel> _activeChannels = new List<Channel>();
    private Coroutine _babbleCoroutine;
    
    private Human _currentHuman;
    private Transform _currentSource;
    
    private BabbleType _currentBabbleType;
    private float _currentPitch;
    private float _currentVolume;

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
        Vector3 position = _currentSource.position;
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

    public void StopBabbleRoutine()
    {
        if (_babbleCoroutine != null)
        {
            StopCoroutine(_babbleCoroutine);
            _babbleCoroutine = null;
        }

        _activeChannels.Clear();
    }

    public void StartBabbleRoutine(string babbleInput, BabbleType babbleType, Human human)
    {
        StopBabbleRoutine();
        
        _currentBabbleType = babbleType;
        _currentHuman = human;
        _currentSource = babbleType != BabbleType.PhoneSpeech ? _currentHuman?.lookAtThisTransform : GetPlayerPhoneTransform();

        if (_currentSource == null)
        {
            _currentSource = Player.Instance?.transform;
        }
        
        float genderScale = _currentHuman != null ? _currentHuman.genderScale : 0.5f;
        _currentPitch = Mathf.Lerp(BabblerConfig.MinimumPitch, BabblerConfig.MaximumPitch, 1f - genderScale);
        
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
        foreach (PhoneticSound phonetic in _phoneticsToBabble)
        {
            FMODReferences.System.playSound(phonetic.Sound, FMODReferences.GetChannelGroup(_currentBabbleType), false, out Channel channel);
            
            channel.setPitch(_currentPitch);
            channel.setVolume(_currentVolume);
        
            SetChannelPosition(_currentSource.position, channel);
            FMODReferences.System.update();

            _activeChannels.Add(channel);
            
            yield return new WaitForSeconds(Mathf.Max(0f, phonetic.Length - BabblerConfig.SyllableSpeed));
        }
        
        // Releasing will set the GameObject inactive, which will trigger StopBabbleRoutine.
        BabblerPool.ReleaseBabbler(this);
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
        // These all appear to be valid at different times so search for everything.
        GameObject receiver = Player.Instance.interactingWith?.controller?.phoneReciever ??
                              Player.Instance.phoneInteractable?.controller?.phoneReciever ??
                              Player.Instance.answeringPhone?.interactable?.controller?.phoneReciever;

        return receiver?.transform;
    }
}