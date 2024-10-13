using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimationFunctions : MonoBehaviour
{
    MonsterBehaviour _behaviour;
    void Start()
    {
        _behaviour = GetComponentInParent<MonsterBehaviour>();
    }

    public void Attack()
    {
        _behaviour.Swing();
    }

    public void Death()
    {
        _behaviour.Death();
    }
}
