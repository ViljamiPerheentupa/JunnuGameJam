using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] AudioSource[] _spawnSounds;

    Transform _monsterParent;

    private void Start()
    {
        _monsterParent = GameObject.Find("MONSTERS").transform;
    }
    public void SpawnMonster(GameObject monster)
    {
        _spawnSounds[Random.Range(0, _spawnSounds.Length)].Play();
        Instantiate(monster, transform.position, transform.rotation, _monsterParent);
    }
}
