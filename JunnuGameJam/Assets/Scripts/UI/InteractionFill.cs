using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractionFill : MonoBehaviour
{
    public static InteractionFill Instance;

    [SerializeField] private float _moveDistance = 200f;
    [SerializeField] private float _appearTime = 0.2f;
    private bool _isAppearing;
    private float _appearTick;

    private RectTransform _rect;
    private Vector2 _startPosition;
    private Vector2 _endPosition;

    private CanvasGroup _fillCanvas;
    [SerializeField] private Image _fillImage;

    float _lastFrameFill;
    float _noProgressTime = 0.1f;
    float _noProgressTick;
    bool _noProgress;


    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        } else
        {
            Debug.LogError("Multiple instances of " + name + " were detected");
            Destroy(gameObject);
        }
        _rect = GetComponent<RectTransform>();
        _endPosition = _rect.anchoredPosition;
        _startPosition = _rect.anchoredPosition;
        _startPosition.y -= _moveDistance;
        _fillCanvas = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isAppearing)
        {
            float _appear = (Time.time - _appearTick) / _appearTime;
            float newAlpha = Mathf.Lerp(0f, 1f, _appear);
            _fillCanvas.alpha = newAlpha;
            Vector2 newPosition = Vector2.Lerp(_startPosition, _endPosition, _appear);
            _rect.anchoredPosition = newPosition;
            if(newAlpha == 1f)
            {
                _isAppearing = false;
            }
        }
        if (_lastFrameFill == _fillImage.fillAmount && !_noProgress)
        {
            _noProgress = true;
            _noProgressTick = Time.time;
        } else if(_lastFrameFill != _fillImage.fillAmount && _noProgress)
        {
            _noProgress = true;
        }
        if (_noProgress && Time.time - _noProgressTick >= _noProgressTime)
        {
            if(_lastFrameFill == _fillImage.fillAmount)
            {
                HideFiller();
            }
        }
        _lastFrameFill = _fillImage.fillAmount;
    }

    public void StartFiller()
    {
        if (!_isAppearing)
        {
            _rect.anchoredPosition = _startPosition;
            _isAppearing = true;
            _appearTick = Time.time;
        }
    }

    public void SetFillAmount(float amount)
    {
        _fillImage.fillAmount = amount;
        if(_fillCanvas.alpha == 0f)
        {
            StartFiller();
        }
    }

    public void HideFiller()
    {
        _fillCanvas.alpha = 0f;
    }
}
