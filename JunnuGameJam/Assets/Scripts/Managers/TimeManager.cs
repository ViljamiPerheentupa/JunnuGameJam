using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    private float seconds = 0f;
    private int currentDay;
    public DateAndTime time;
    [SerializeField] float _timeMultiplier;
    float _timeTick;

    bool _daySwitch;

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
        _timeTick = Time.time;
        currentDay = time.days;
    }

    void Update()
    {
        TimePassage();
    }

    void TimePassage()
    {
        if(Time.time - _timeTick >= 1f)
        {
            seconds += 1f * _timeMultiplier;
            _timeTick = Time.time;
            if (seconds >= 60f)
            {
                seconds -= 60f;
                time.AddMinutes(1);
                if(!_daySwitch && time.days != currentDay)
                {
                    SwitchDay();
                }
            }
        }
    }

    void SwitchDay()
    {
        _daySwitch = true;
    }

    public string CurrentTime()
    {
        string _hours = string.Empty;
        string _minutes = string.Empty;
        if(time.hours < 10)
        {
            _hours = "0" + time.hours;
        } else
        {
            _hours = time.hours.ToString();
        }
        if(time.minutes < 10)
        {
            _minutes = "0" + time.minutes;
        } else
        {
            _minutes = time.minutes.ToString();
        }
        return _hours + ":" + _minutes;
    }
}

[Serializable]
public struct DateAndTime
{
    public int days;
    public int hours;
    public int minutes;

    public void SetTime(int _hours, int _minutes)
    {
        hours = _hours;
        minutes = _minutes;
    }

    public void AddMinutes(int amount)
    {
        while(minutes + amount > 59)
        {
            amount -= 60;
            AddHours(1);
        }
        minutes += amount;
    }

    public void AddHours(int amount)
    {
        while(hours + amount > 23)
        {
            amount -= 23;
            days++;
        }
        hours += amount;
    }
}
