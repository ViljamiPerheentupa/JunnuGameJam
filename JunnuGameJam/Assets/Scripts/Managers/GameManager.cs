using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int _currentDay;
    Transform _monstersParent;

    InputAction _menuAction;
    bool _gamePaused;
    [SerializeField] CanvasGroup _menuGroup;
    [SerializeField] CanvasGroup _introGroup;
    [SerializeField] Volume _pauseVolume;
    float _introDuration = 15f;
    float _introTick;

    bool _intro = true;

    int _enemiesKilled = 0;

    [SerializeField] CanvasGroup _deathGroup;
    [SerializeField] TMP_Text _deathText;

    [SerializeField] CanvasGroup _winGroup;

    [SerializeField] AudioMixer SFXMixer;

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
        _menuAction = InputSystem.actions.FindAction("Escape");
    }

    private void Start()
    {
        _introTick = Time.time;
        _introGroup.alpha = 1f;
        SFXMixer.SetFloat("SFXVolume", -80f);
    }


    private void Update()
    {
        if (_intro)
        {
            if(Time.time - _introTick >= _introDuration)
            {
                _intro = false;
                _introGroup.alpha = 0f;
                StartGame();
            }
            if (_menuAction.WasPressedThisFrame())
            {
                _intro = false;
                _introGroup.alpha = 0f;
                StartGame();
                return;
            }
        }
        if (_menuAction.WasPressedThisFrame() && !_gamePaused)
        {
            PauseGame();
        }
        if (_menuAction.WasPressedThisFrame() && _gamePaused)
        {
            ResumeGame();
        }
    }

    void PauseGame()
    {
        _menuGroup.alpha = 1f;
        _menuGroup.interactable = true;
        TimeManager.Instance.PauseTime();
        _pauseVolume.weight = 1f;
        Time.timeScale = 0f;
        CameraController.Instance.cameraState = CameraState.Locked;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        _menuGroup.alpha = 0f;
        _menuGroup.interactable = false;
        _pauseVolume.weight = 0f;
        TimeManager.Instance.ResumeTime();
        CameraController.Instance.cameraState = CameraState.Restricted;
    }

    public void StartGame()
    {
        _currentDay = 0;
        SpookyManager.Instance.StartSpooky();
        TimeManager.Instance.StartTime();
        StartDay();
        SFXMixer.SetFloat("SFXVolume", 0f);
    }

    public void StartDay()
    {
        if(_currentDay < 6)
        {
            EventManager.Instance.StartWave(_currentDay);
        } else
        {
            EventManager.Instance.Day7();
        }
    }

    public void EndDay()
    {
        RemoveMonsters();
        if(_currentDay < 6)
        {
            _currentDay++;
        } else
        {
            PlayerWin();
        }

    }

    public void PlayerDeath()
    {
        Time.timeScale = 0f;
        _deathGroup.alpha = 1f;
        _deathGroup.blocksRaycasts = true;
        _deathGroup.interactable = true;
        _deathText.text = "having survived " + (_currentDay + 1) + " days...";
    }

    public void PlayerWin()
    {
        Time.timeScale = 0f;
        _winGroup.alpha = 1f;
        _winGroup.blocksRaycasts = true;
        _winGroup.interactable = true;
    }

    void RemoveMonsters()
    {
        var _monsters = _monstersParent.childCount;
        for (int i = 0; i < _monsters; i++)
        {
            Destroy(_monstersParent.GetChild(i).gameObject);
        }
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadSceneAsync(0);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void AddKill()
    {
        _enemiesKilled++;
    }
}
