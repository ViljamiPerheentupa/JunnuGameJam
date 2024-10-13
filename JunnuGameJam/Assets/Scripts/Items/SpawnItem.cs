using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : MonoBehaviour
{
    [SerializeField] GameObject[] _itemPrefab;
    Transform _entities;

    private void Start()
    {
        _entities = GameObject.Find("ENTITIES").transform;
    }
    public void Spawn()
    {
        Instantiate(_itemPrefab[Random.Range(0, _itemPrefab.Length)], transform.position + Vector3.up * 0.5f, transform.rotation);
    }
}
