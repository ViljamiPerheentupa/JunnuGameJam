using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class ItemInteraction : MonoBehaviour
{
    private List<IInteractable> _objectsInRadius = new List<IInteractable>();
    private IInteractable _selectedObject;
    [SerializeField] private LayerMask _mask;
    private float _radius;

    private void Start()
    {
        _radius = 2.5f;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            IInteractable newObject = other.GetComponent<IInteractable>();
            if (!_objectsInRadius.Contains(newObject))
            {
                _objectsInRadius.Add(newObject);
                Debug.Log("Added " + newObject);
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
                Debug.Log("Removed " + leavingObject);
            }
        }
    }

    private void Update()
    {
        if(_objectsInRadius.Count > 0)
        {
            RaycastHit hit;
            if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, _radius, _mask))
            {
                IInteractable hitObject = hit.collider.GetComponent<IInteractable>();
                if(hitObject != null)
                {
                    if (_objectsInRadius.Contains(hitObject))
                    {
                        _selectedObject = hitObject;
                    }
                    _selectedObject.Interact();
                }
            } else
            {
                if(_selectedObject != null)
                {
                    _selectedObject = null;
                }
            }
        }
    }
}
