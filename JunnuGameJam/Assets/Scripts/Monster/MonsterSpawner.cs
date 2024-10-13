using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    Transform _monsterParent;

    private void Start()
    {
        _monsterParent = GameObject.Find("MONSTERS").transform;
    }
    public void SpawnMonster(GameObject monster)
    {
        Instantiate(monster, transform.position, transform.rotation, _monsterParent);
    }
}
