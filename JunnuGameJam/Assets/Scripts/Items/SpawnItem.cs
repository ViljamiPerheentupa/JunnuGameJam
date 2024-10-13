using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : MonoBehaviour
{
    [SerializeField] GameObject _itemPrefab;
    public void Spawn()
    {
        Instantiate(_itemPrefab, transform.position, transform.rotation);
    }
}
