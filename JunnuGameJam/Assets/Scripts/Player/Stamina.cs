using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    public static Stamina Instance;

    [SerializeField] private float _maxStamina;
    private float _currentStamina;
    [SerializeField] private float _drainPerSecond;
    bool _usingStamina = false;
    float _staminaTick, _uiShowTick, _uiHideTick, _uiTick;

    [SerializeField] private CanvasGroup _staminaGroup;
    [SerializeField] private Image _staminaFill;

    [SerializeField] private float _staminaBarAppearTime = 0.5f;
    [SerializeField] private float _staminaBarShowTime = 5f;
    [SerializeField] private float _staminaBarHideTime = 2f;
    [SerializeField] private float _staminaBarAlwaysShowAmount = 0.33f;
    bool _uiVisible, _uiShowingUp, _uiHiding, _uiShowAfterUse;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        } else
        {
            Debug.LogError("Multiple instances of " + name + " detected.");
            Destroy(gameObject);
        }
        _currentStamina = _maxStamina;
    }
    // Update is called once per frame
    void Update()
    {
        if( _usingStamina)
        {
            UseStamina();
            if(!_uiVisible && !_uiShowingUp)
            {
                _uiShowingUp = true;
                _uiShowTick = Time.time;
                _uiHiding = false;
                _uiShowAfterUse = false;
            }
        }
        if (_uiShowingUp)
        {
            ShowUI();
        }
        if (_currentStamina / _maxStamina > _staminaBarAlwaysShowAmount)
        {
            if ((!_usingStamina && !_uiShowAfterUse) && _uiVisible || _uiShowingUp)
            {
                _uiShowAfterUse = true;
                _uiTick = Time.time;
            }
            if (_uiShowAfterUse)
            {
                UITime();
            }
            if (_uiHiding)
            {
                HideUI();
            }
        }
    }

    void ShowUI()
    {
        float alphaAmount = Mathf.Lerp(0f, 1.0f, (Time.time - _uiShowTick) / _staminaBarAppearTime);
        _staminaGroup.alpha = alphaAmount;
        if(alphaAmount >= 1.0f)
        {
            _uiVisible = true;
            _uiShowingUp = false;
        }
    }

    void UITime()
    {
        if(Time.time >= _uiTick + _staminaBarShowTime)
        {
            _uiHiding = true;
            _uiShowAfterUse = false;
            _uiHideTick = Time.time;
        }
    }

    void HideUI()
    {
        float alphaAmount = Mathf.Lerp(1f, 0f, (Time.time - _uiHideTick) / _staminaBarHideTime);
        _staminaGroup.alpha = alphaAmount;
        if (alphaAmount == 0f)
        {
            _uiVisible = false;
            _uiHiding = false;
        }
    }

    void UseStamina()
    {
        if(Time.time >= _staminaTick + 1.0f)
        {
            _staminaTick = Time.time;
            ReduceStamina(_drainPerSecond);
        }
    }

    void UpdateUI()
    {
        _staminaFill.fillAmount = _currentStamina / _maxStamina;
    }

    public bool CanRun()
    {
        return _currentStamina > 0;
    }

    public void AddStamina(float _amount)
    {
        if(_currentStamina + _amount > _maxStamina)
        {
            _currentStamina = _maxStamina;
        } else
        {
            _currentStamina += _amount;
        }
        UpdateUI();
        if(!_uiVisible && !_uiShowingUp)
        {
            _uiShowingUp = true;
            _uiShowTick = Time.time;
        }
    }

    public void ReduceStamina(float _amount)
    {
        if(_currentStamina - _amount < 0)
        {
            _currentStamina = 0;
        } else
        {
            _currentStamina -= _amount;
        }
        UpdateUI();
        if (!_uiVisible && !_uiShowingUp)
        {
            _uiShowingUp = true;
            _uiShowTick = Time.time;
        }
    }

    public void SetStaminaUseState(bool _state)
    {
        _usingStamina = _state;
        if(_usingStamina)
        {
            _staminaTick = Time.time;
        }
    }
}
