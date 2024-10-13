using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RequiresPrompt : MonoBehaviour
{
    public static RequiresPrompt Instance;
    [SerializeField] private float _appearDuration;
    private float _leaveDuration = 0.25f;
    [SerializeField] private float _stayDuration;
    private bool _appearing, _staying, _leaving;
    private float _appearTick, _stayTick, _leaveTick;
    [SerializeField] private TMP_Text _promptText;
    [SerializeField] private CanvasGroup _canvasGroup;
    void Awake()
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
        if (_appearing)
        {
            float newAlpha = Mathf.Lerp(0f, 1f, (Time.time - _appearTick) / _appearDuration);
            _canvasGroup.alpha = newAlpha;
            if(newAlpha == 1f)
            {
                _appearing = false;
                _staying = true;
                _stayTick = Time.time;
            }
        }
        if (_staying)
        {
            if(Time.time - _stayTick >= _stayDuration)
            {
                _staying = false;
                _leaving = true;
                _leaveTick = Time.time;
            }
        }
        if (_leaving)
        {
            float newAlpha = Mathf.Lerp(1f, 0f, (Time.time - _leaveTick) / _leaveDuration);
            _canvasGroup.alpha = newAlpha;
            if(newAlpha == 0f)
            {
                _leaving = false;
                _promptText.text = string.Empty;
            }
        }
    }

    public void SetPrompt(string _prompt)
    {
        if(_promptText.text != _prompt)
        {
            _promptText.text = _prompt;
        }
        if(!_appearing && !_staying || _leaving)
        {
            _leaving = false;
            _appearing = true;
            _appearTick = Time.time;
            return;
        } else if (_staying)
        {
            _stayTick = Time.time;
        }
    }
}
