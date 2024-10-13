using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int _currentDay;
    Transform _monstersParent;

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

    public void StartDay()
    {
        
    }

    public void EndDay()
    {
        RemoveMonsters();
    }

    void RemoveMonsters()
    {
        var _monsters = _monstersParent.childCount;
        for (int i = 0; i < _monsters; i++)
        {
            Destroy(_monstersParent.GetChild(i).gameObject);
        }
    }
}
