using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField] private MinMaxInt[] waveSpawnAmounts;
    [SerializeField] private MonsterSpawner[] spawners;
    [SerializeField] private float _dayLength = 300f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}

[Serializable]
public struct MinMaxInt
{
    public int minValue;
    public int maxValue;
}
