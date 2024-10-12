using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnvironmentalHazard : MonoBehaviour
{
    [SerializeField] private float _damagePerTick;
    [SerializeField] private float _timeBetweenTicks;
    bool _ticking;
    float _lastDamageTick;
    [SerializeField] UnityEvent _onEnterEvent;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_ticking)
        {
            Health.Instance.ReduceHealth(_damagePerTick);
            _ticking = true;
            _lastDamageTick = Time.time;
            _onEnterEvent?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player") && _ticking)
        {
            _ticking = false;
        }
    }

    void Update()
    {
        if(_ticking)
        {
            if(Time.time -  _lastDamageTick >= _timeBetweenTicks)
            {
                Health.Instance.ReduceHealth(_damagePerTick);
                _lastDamageTick = Time.time;
            }
        }
    }
}
