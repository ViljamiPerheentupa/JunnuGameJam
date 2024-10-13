using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
    float _introDuration = 15f;
    float _introTick;

    bool _intro = true;

    int _enemiesKilled = 0;

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
        }
        if (_menuAction.WasPressedThisFrame() && !_gamePaused)
        {
            PauseGame();
        } else if (_menuAction.WasPressedThisFrame() && _gamePaused)
        {
            ResumeGame();
        }
    }

    void PauseGame()
    {
        _menuGroup.alpha = 1f;
        _menuGroup.interactable = true;
        TimeManager.Instance.PauseTime();
        Time.timeScale = 0f;
    }

    void ResumeGame()
    {
        Time.timeScale = 1f;
        _menuGroup.alpha = 0f;
        _menuGroup.interactable = false;
        TimeManager.Instance.ResumeTime();
    }

    public void StartGame()
    {
        _currentDay = 0;
        SpookyManager.Instance.StartSpooky();
        TimeManager.Instance.StartTime();
        StartDay();
    }

    public void StartDay()
    {
        if(_currentDay < 7)
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
        if(_currentDay < 7)
        {
            _currentDay++;
        } else
        {
            //WIN
        }

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
