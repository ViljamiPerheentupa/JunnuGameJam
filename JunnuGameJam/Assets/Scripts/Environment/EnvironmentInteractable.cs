using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class EnvironmentInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string objectName;
    [SerializeField] private string prompt;
    [SerializeField] private string requiredItem;
    [SerializeField] private bool _depleteRequiredItem;
    [SerializeField] private InteractType interactType;
    [SerializeField] private float _holdDuration;
    [SerializeField] private bool _progressSaved;
    [SerializeField] private bool _repeatable;
    [SerializeField] private UnityEvent _interactEvent;
    private float _holdTime;
    private float _lastFrameHoldTime;
    private float _progressCheckTime = 0.1f;
    private float _noProgressTick;
    private bool _noProgress;

    void Update()
    {
        if(!_progressSaved)
        {
            if(_lastFrameHoldTime == _holdTime && !_noProgress)
            {
                _noProgress = true;
                _noProgressTick = Time.time;
            } else if(_lastFrameHoldTime != _holdTime && !_noProgress)
            {
                _noProgress = false;
            }
            if (_noProgress && Time.time - _noProgressTick >= _progressCheckTime)
            {
                if(_lastFrameHoldTime == _holdTime)
                {
                    _holdTime = 0f;
                }
            }
            _lastFrameHoldTime = _holdTime;
        }
    }
    public void Interact()
    {
        if(requiredItem != string.Empty)
        {
            ItemThrower _thrower = FindFirstObjectByType<ItemThrower>();
            if(_thrower.heldObject == null)
            {
                RequiresPrompt.Instance.SetPrompt("Requires " + requiredItem + ".");
                return;
            }
            if (_thrower.heldObject != null && _thrower.heldObject.GetComponent<IInteractable>().ObjectName() != requiredItem)
            {
                RequiresPrompt.Instance.SetPrompt("Requires " + requiredItem + ".");
                return;
            }
        }
        if(interactType == InteractType.HoldEnvironmental)
        {
            _holdTime += Time.deltaTime;
            InteractionFill.Instance.SetFillAmount(_holdTime / _holdDuration);
            if(_holdTime >= _holdDuration)
            {
                if (_repeatable)
                {
                    _holdTime = 0f;
                } else
                {
                    InteractionFill.Instance.HideFiller();
                    ItemInteraction.Instance.DeselectObject();
                }
                _interactEvent.Invoke();
                if(_depleteRequiredItem && requiredItem != string.Empty)
                {
                    ItemInteraction.Instance.DeleteHeldObject();
                }
            }
        } else if(interactType == InteractType.Environmental)
        {
            _interactEvent.Invoke();
            if (_depleteRequiredItem && requiredItem != string.Empty)
            {
                ItemInteraction.Instance.DeleteHeldObject();
            }
        }
    }

    public string ObjectName()
    {
        return objectName;
    }

    public string Prompt()
    {
        return prompt;
    }
    public InteractType InteractionType()
    {
        return interactType;
    }

    public void DebugInteraction()
    {
        Debug.Log("Success");
    }
}
