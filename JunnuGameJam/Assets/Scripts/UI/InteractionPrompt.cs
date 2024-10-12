using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionPrompt : MonoBehaviour
{
    public static InteractionPrompt Instance { get; private set; }

    [SerializeField] private float _appearDuration;
    [SerializeField] private float _disappearDuration;
    bool _appearing, _disappearing;
    float _appearTick, _disappearTick, _oldAlpha;

    [SerializeField] private TMP_Text _promptText;
    [SerializeField] private CanvasGroup _promptCanvasGroup;

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

    private void Update()
    {
        if(_appearing)
        {
            float newAlpha = Mathf.Lerp(_oldAlpha, 1f, (Time.time - _appearTick) / _appearDuration);
            _promptCanvasGroup.alpha = newAlpha;
            if(newAlpha == 1f)
            {
                _appearing = false;
            }
        }
        else if(_disappearing)
        {
            float newAlpha = Mathf.Lerp(_oldAlpha, 0f, (Time.time - _disappearTick) / _disappearDuration);
            _promptCanvasGroup.alpha = newAlpha;
            if (newAlpha == 0f)
            {
                _disappearing = false;
                _promptText.text = string.Empty;
            }
        }
    }


    public void ShowPrompt(string _prompt)
    {
        _promptText.text = _prompt;
        if (_promptCanvasGroup.alpha != 1f && !_appearing)
        {
            _oldAlpha = _promptCanvasGroup.alpha;
            _appearing = true;
            _appearTick = Time.time;
        }
    }

    public void HidePrompt()
    {
        _appearing = false;
        if (_promptCanvasGroup.alpha != 0f && !_disappearing)
        {
            _oldAlpha = _promptCanvasGroup.alpha;
            _disappearing = true;
            _disappearTick = Time.time;
        }
    }

    public bool PromptVisible()
    {
        return _promptText.text != string.Empty;
    }
}
