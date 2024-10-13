using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Health : MonoBehaviour
{
    public static Health Instance;
    [SerializeField] float _maxHealth;
    [SerializeField] float _regenCooldown;
    [SerializeField] float _regenRate;
    [SerializeField] Volume _volume;
    public float health { get; private set; }

    private float _lastDamageTick;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple instances of " + name + " were detected.");
            Destroy(gameObject);
        }
        health = _maxHealth;
    }

    void Update()
    {
        if (!FullHealth())
        {
            float _regen = Time.time - _lastDamageTick;
            if(_regen >= _regenCooldown)
            {
                IncreaseHealth(Time.deltaTime * _regenRate);
            }
            _volume.weight = 1 - (health / _maxHealth);
        }
    }

    public void ReduceHealth(float _amount)
    {
        health -= _amount;
        if (health < 0)
        {
            GameManager.Instance.PlayerDeath();
            return;
        }
        _lastDamageTick = Time.time;
        Debug.Log(health);
    }

    public void IncreaseHealth(float _amount)
    {
        {
            if (health + _amount > _maxHealth)
            {
                health = _maxHealth;
                _volume.weight = 0f;
            }
            else
            {
                health += _amount;
            }
            Debug.Log(health);
        }
    }

    bool FullHealth()
    {
        return health == _maxHealth;
    }
}
