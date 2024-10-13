using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    [SerializeField] private MinMaxInt[] waveSpawnAmounts;
    [SerializeField] private MonsterSpawner[] spawners;
    [SerializeField] private float _dayLength = 300f;
    bool _spawning, _lastDay;
    int currentSpawnAmount;
    int _spawned;
    float _spawnTick;
    float _currentSpawnTime;
    [SerializeField] GameObject[] _monsterPrefabs;
    [SerializeField] private float _lastDaySpawnTime = 12.5f;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        } else
        {
            Debug.LogError("Multiple instances of " + name + " were detected.");
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (_spawning)
        {
            if(_spawned < currentSpawnAmount)
            {
                if(Time.time - _spawnTick >= _currentSpawnTime)
                {
                    SpawnMonster();
                }
            } else
            {
                _spawning = false;
            }
        }
        if(_lastDay)
        {
            if(Time.time - _spawnTick >= _lastDaySpawnTime)
            {
                SpawnDay7Monster();
            }
        }
        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            Day7();
        }
    }

    public void StartWave(int index)
    {
        currentSpawnAmount = SpawnAmount(index);
        _spawned = 0;
        _spawning = true;
        SpawnMonster();
    }

    public void Day7()
    {
        currentSpawnAmount = 1000;
        _spawned = 0;
        _lastDay = true;
    }

    void SpawnDay7Monster()
    {
        RandomizeSpawner().SpawnMonster(RandomizeMonster());
        _spawned++;
        _spawnTick = Time.time;
    }

    void SpawnMonster()
    {
        RandomizeSpawner().SpawnMonster(RandomizeMonster());
        _spawned++;
        _currentSpawnTime = TimeBetweenSpawns(currentSpawnAmount);
        _spawnTick = Time.time;
    }

    int SpawnAmount(int index)
    {
        int i = UnityEngine.Random.Range(waveSpawnAmounts[index].minValue, waveSpawnAmounts[index].maxValue + 1);
        return i;
    }

    float TimeBetweenSpawns(int amount)
    {
        float baseTime = _dayLength / amount;
        float variableTime = UnityEngine.Random.Range(-baseTime / 10f, baseTime / 10f);
        return baseTime + variableTime;
    }

    MonsterSpawner RandomizeSpawner()
    {
        int i = UnityEngine.Random.Range(0, spawners.Length);
        return spawners[i];
    }

    GameObject RandomizeMonster()
    {
        int i = UnityEngine.Random.Range(0, _monsterPrefabs.Length);
        return _monsterPrefabs[i];
    }
}

[Serializable]
public struct MinMaxInt
{
    public int minValue;
    public int maxValue;
}
