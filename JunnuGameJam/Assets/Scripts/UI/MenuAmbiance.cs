using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MenuAmbiance : MonoBehaviour
{
    [SerializeField] AudioSource _ambiance;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void MenuAmbianceState(bool _state)
    {
        if (_state)
        {
            _ambiance.Play();
        } else
        {
            _ambiance.Stop();
        }
    }
}
