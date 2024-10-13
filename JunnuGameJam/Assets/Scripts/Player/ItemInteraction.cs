using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public enum InteractType { Held = 0, Environmental = 1, HoldEnvironmental = 2 }
public class ItemInteraction : MonoBehaviour
{
    public static ItemInteraction Instance;

    private List<IInteractable> _objectsInRadius = new List<IInteractable>();
    private IInteractable _selectedObject;
    [SerializeField] private LayerMask _mask;
    [SerializeField] private Transform _itemHolder;
    ItemThrower _thrower;

    InputAction _interactAction;

    TMP_Text _interactPrompt;

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
        _interactAction = InputSystem.actions.FindAction("Interact");
        _thrower = _itemHolder.GetComponent<ItemThrower>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            IInteractable newObject = other.GetComponent<IInteractable>();
            if (!_objectsInRadius.Contains(newObject))
            {
                _objectsInRadius.Add(newObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            IInteractable leavingObject = other.GetComponent<IInteractable>();
            if (_objectsInRadius.Contains(leavingObject))
            {
                _objectsInRadius.Remove(leavingObject);
            }
        }
    }

    private void Update()
    {
        if(_objectsInRadius.Count > 0)
        {
            RaycastHit hit;
            if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, _mask))
            {
                IInteractable hitObject = hit.collider.GetComponent<IInteractable>();
                if(hitObject != null)
                {
                    if (_objectsInRadius.Contains(hitObject))
                    {
                        _selectedObject = hitObject;
                    }
                } else
                {
                    _selectedObject = null;
                }
            } else
            {
                _selectedObject = null;
            }
        } else
        {
            _selectedObject = null;
        }
        if(_selectedObject != null)
        {
            UIPrompt(_selectedObject.InteractionType());
            if(_selectedObject.InteractionType() == InteractType.HoldEnvironmental && _interactAction.IsPressed())
            {
                _selectedObject.Interact();
            } else if (_selectedObject.InteractionType() != InteractType.HoldEnvironmental && _interactAction.WasReleasedThisFrame())
            {
                _selectedObject.Interact();
            }
        }
        if(_selectedObject == null && InteractionPrompt.Instance.PromptVisible())
        {
            InteractionPrompt.Instance.HidePrompt();
        }
    }

    void UIPrompt(InteractType _type)
    {
        string _prompt = string.Empty;
        if(_type == InteractType.Held)
        {
            _prompt = "Press E to " + _selectedObject.Prompt() + " " + _selectedObject.ObjectName();
        }
        if(_type == InteractType.Environmental)
        {
            _prompt = "Press E to " + _selectedObject.Prompt() + " " + _selectedObject.ObjectName();
        }
        if(_type == InteractType.HoldEnvironmental)
        {
            _prompt = "Hold E to " + _selectedObject.Prompt() + " " + _selectedObject.ObjectName();
        }
        InteractionPrompt.Instance.ShowPrompt(_prompt);
    }

    public void PickupItem(GameObject _obj)
    {
        _thrower.GrabObject(_obj);
        AudioFW.Instance.PlaySound("PickUp");
    }

    public void DeselectObject()
    {
        _selectedObject = null;
    }

    public void DeleteHeldObject()
    {
        _thrower.DestroyHeldObject();
        DeselectObject();
    }
}
