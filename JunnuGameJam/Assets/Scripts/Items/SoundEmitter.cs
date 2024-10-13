using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    [SerializeField] private AudioSource[] _audio;

    public void PlaySound()
    {
        _audio[RandomIndex()].Play();
    }

    int RandomIndex()
    {
        return Random.Range(0, _audio.Length);
    }
}
