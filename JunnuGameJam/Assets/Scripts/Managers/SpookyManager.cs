using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpookyManager : MonoBehaviour
{
    public static SpookyManager Instance;

    [SerializeField] string[] _ambiances;
    [SerializeField] string[] _noises;
    [SerializeField] MinMaxFloat _noiseMinMax;
    float _noiseCooldown;
    public bool _noisesOn = true;
    private float _noiseTick;
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        } else
        {
            Debug.LogError("Multiple instances of " + name + " were detected.");
            Destroy(gameObject);
        }
    }

    public void RandomizeSpooky()
    {
        _noiseCooldown = RandomDuration();
        _noiseTick = Time.time;
        RandomizeAmbiance();
    }
    void Update()
    {
        if (_noisesOn)
        {
            if(Time.time - _noiseTick >= _noiseCooldown)
            {
                int index = RandomIndex(_noises.Length);
                AudioFW.Instance.RandomizeSoundVolume(_noises[index], 0.75f, 1.00f);
                AudioFW.Instance.RandomizeSoundPan(_noises[index]);
                AudioFW.Instance.PlaySound(_noises[index]);
                _noiseTick = Time.time;
                _noiseCooldown = RandomDuration();
            }
        }
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            RandomizeSpooky();
        }
    }

    void RandomizeAmbiance()
    {
        int _ambiancesSkipped = 0;
        for (int i = 0; i < _ambiances.Length; i++)
        {
            if (UnityEngine.Random.Range(0, 2) < 1)
            {
                AudioFW.Instance.RandomizeSoundVolume(_ambiances[i], 0.6f, 0.9f);
                AudioFW.Instance.PlaySound(_ambiances[i]);
            }
            else
            {
                AudioFW.Instance.StopSound(_ambiances[i]);
                _ambiancesSkipped++;
            }
        }
        if(_ambiancesSkipped == _ambiances.Length)
        {
            int index = UnityEngine.Random.Range(0, _ambiances.Length);
            AudioFW.Instance.RandomizeSoundVolume(_ambiances[index], 0.6f, 0.9f);
            AudioFW.Instance.PlaySound(_ambiances[index]);
        }
    }

    int RandomIndex(int maxValue)
    {
        return UnityEngine.Random.Range(0, maxValue);
    }

    float RandomDuration()
    {
        return UnityEngine.Random.Range(_noiseMinMax.minValue, _noiseMinMax.maxValue);
    }
}

[Serializable]
public struct MinMaxFloat
{
    public float minValue;
    public float maxValue;
}
