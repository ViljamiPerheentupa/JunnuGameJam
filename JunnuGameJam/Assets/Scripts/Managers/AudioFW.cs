using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFW : MonoBehaviour
{
    public static AudioFW Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
    }

    public void RandomizeSoundVolume(string path, float minVolume, float maxVolume)
    {
        AudioSource _source = transform.Find(path).GetComponent<AudioSource>();
        _source.volume = Random.Range(minVolume, maxVolume);
    }
    public void RandomizeSoundPan(string path)
    {
        AudioSource _sound = transform.Find(path).GetComponent<AudioSource>();
        _sound.panStereo = Random.Range(-1.0f, 1.0f);
    }
    public void PlaySound(string path)
    {
        transform.Find(path).GetComponent<AudioSource>().Play();
    }

    public void StopSound(string path)
    {
        transform.Find(path).GetComponent<AudioSource>().Stop();
    }
}
