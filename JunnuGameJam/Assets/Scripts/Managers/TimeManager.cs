using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    private float seconds = 0f;
    private int currentDay;
    public DateAndTime time;
    [SerializeField] private DateAndTime _startTime;
    [SerializeField] float _timeMultiplier;
    float _timeTick;

    bool _daySwitch, _fadeIn, _fadeOut, _timePassing, _blackScreen;
    [SerializeField] float _fadeInDuration;
    [SerializeField] float _fadeOutDuration;
    [SerializeField] float _blackScreenDuration;
    [SerializeField] float _daySwitchDuration;
    float _fadeInTick, _fadeOutTick, _switchTick, _blackScreenTick;

    [SerializeField] CanvasGroup _blackScreenCanvas;
    [SerializeField] TMP_Text _dayText;

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
        SwitchDay();
        if(Keyboard.current.enterKey.wasPressedThisFrame)
        {
            StartTime();
        }
    }

    void TimePassage()
    {
        if(_timePassing)
        {
            if (Time.time - _timeTick >= 1f)
            {
                seconds += 1f * _timeMultiplier;
                _timeTick = Time.time;
                while (seconds >= 60f)
                {
                    seconds -= 60f;
                    time.AddMinutes(1);
                    if (!_daySwitch && time.days != currentDay)
                    {
                        _fadeIn = true;
                        _fadeInTick = Time.unscaledTime;
                        currentDay++;
                        PauseTime();
                    }
                }
            }
        }
    }

    public void StartTime()
    {
        time = _startTime;
        currentDay = time.days;
        _dayText.text = "Day 1";
        _blackScreenCanvas.alpha = 1;
        _daySwitch = true;
        _switchTick = Time.unscaledTime;
    }

    public void SwitchDay()
    {
        if (_fadeIn)
        {
            float newAlpha = Mathf.Lerp(0f, 1f, (Time.unscaledTime - _fadeInTick) / _fadeInDuration);
            _blackScreenCanvas.alpha = newAlpha;
            if(newAlpha == 1f)
            {
                _fadeIn = false;
                _blackScreenTick = Time.unscaledTime;
                _blackScreen = true;
            }
        }
        if (_blackScreen)
        {
            if (Time.unscaledTime - _blackScreenTick >= _blackScreenDuration)
            {
                _blackScreen = false;
                _daySwitch = true;
                _switchTick = Time.unscaledTime;
                _dayText.text = "Day " + currentDay;
            }
        }
        if (_daySwitch)
        {
            Time.timeScale = 0f;
            if (Time.unscaledTime - _switchTick >= _daySwitchDuration)
            {
                _daySwitch = false;
                Time.timeScale = 1f;
                _fadeOut = true;
                _fadeOutTick = Time.unscaledTime;
                time.SetTime(currentDay, _startTime.hours, _startTime.minutes);
                ResumeTime();
            }
        }
        if(_fadeOut)
        {
            float newAlpha = Mathf.Lerp(1f, 0f, (Time.unscaledTime - _fadeOutTick) / _fadeOutDuration);
            _blackScreenCanvas.alpha = newAlpha;
            if(newAlpha == 0f)
            {
                _fadeOut = false;
                _dayText.text = string.Empty;
            }
        }
    }

    public void PauseTime()
    {
        _timePassing = false;
    }

    public void ResumeTime()
    {
        _timePassing = true;
        _timeTick = Time.time;
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

    public void SetTime(int _days, int _hours, int _minutes)
    {
        days = _days;
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
            amount -= 24;
            days++;
        }
        hours += amount;
    }
}
