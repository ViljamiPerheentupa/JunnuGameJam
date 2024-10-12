using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum MonsterState { Moving, Swinging, Backswing, Reeling, }
public class MonsterBehaviour : MonoBehaviour
{
    private Transform _player;
    private NavMeshAgent _agent;
    private MonsterState _state;
    [SerializeField] private float _swingRadius = 1f;
    [SerializeField] LayerMask _mask;
    [SerializeField] private float _damagePerSwing = 50f;
    bool _swinging;
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindWithTag("Player").transform;
        InitiliazeEnemy();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Interactable") && collision.rigidbody.velocity.magnitude > 0.1f)
        {
            Debug.Log("Object was thrown at me :(");
        }
    }

    void Update()
    {
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

    }

    void InitiliazeEnemy()
    {
        _state = MonsterState.Moving;
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
