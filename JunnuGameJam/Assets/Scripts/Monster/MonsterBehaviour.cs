using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum MonsterState { Moving, Swinging, Backswing, Reeling, Death }
public class MonsterBehaviour : MonoBehaviour
{
    [SerializeField] private float _maxHealth;
    private float _health;
    private Transform _player;
    private NavMeshAgent _agent;
    private MonsterState _state;
    [SerializeField] private float _swingRadius = 1f;
    [SerializeField] LayerMask _mask;
    [SerializeField] private float _damagePerSwing = 50f;
    [SerializeField] private float _maxStopDuration = 1.5f;
    float _reelTick, _reelDuration;
    bool _swinging;
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindWithTag("Player").transform;
        InitiliazeEnemy();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Interactable") && collision.rigidbody.velocity.y != 0f)
        {
            float _damageAmount = 15f * collision.rigidbody.mass;
            if(_health - _damageAmount <= 0f)
            {
                _health = 0f;
                _state = MonsterState.Death;
            } else
            {
                _health -= _damageAmount;
                float _stopTime = _damageAmount / _maxHealth * _maxStopDuration;
                Reel(_stopTime);
                Debug.Log("I took " + _damageAmount + " damage and was stopped for " + _stopTime + " seconds :(");
            }
        }
    }

    void Update()
    {
        _agent.isStopped = _state == MonsterState.Moving ? false : true;
        if(_state == MonsterState.Moving)
        {
            _agent.destination = _player.position;
            if (Vector3.Distance(transform.position, _player.position) < _swingRadius)
            {
                _state = MonsterState.Swinging;
            }
        }
        if(_state == MonsterState.Swinging)
        {
            Swing();
        }
        if(_state == MonsterState.Reeling)
        {
            if(Time.time - _reelTick >= _reelDuration)
            {
                _reelDuration = 0f;
                _state = MonsterState.Moving;
            }
        }
    }

    void InitiliazeEnemy()
    {
        _state = MonsterState.Moving;
        _health = _maxHealth;
    }

    void Reel(float _duration)
    {
        _state = MonsterState.Reeling;
        _reelDuration = _duration;
        _reelTick = Time.time;
    }

    void Swing()
    {
        if (Physics.CheckBox(transform.forward + Vector3.forward, Vector3.one, transform.rotation, _mask))
        {
            Health.Instance.ReduceHealth(_damagePerSwing);
        }
        _state = MonsterState.Backswing;
        StartCoroutine(Backswing(2f));
    }

    IEnumerator Backswing(float _duration)
    {
        yield return new WaitForSeconds(_duration);
        _state = MonsterState.Moving;
    }
}
