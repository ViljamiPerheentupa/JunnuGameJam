using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ThrowableObject : MonoBehaviour, IInteractable
{
    public string objectName;
    [SerializeField] private string prompt;
    [SerializeField] LayerMask _layerMask;
    [SerializeField] UnityEvent _onCollisionEvent;

    public InteractType interactType { get; private set; }

    private void OnCollisionEnter(Collision collision)
    {
        if (_layerMask == (_layerMask | (1 << collision.collider.gameObject.layer)))
        {
            _onCollisionEvent.Invoke();
        }
    }

    void Start()
    {
        interactType = InteractType.Held;
    }
    public void Interact()
    {
        ItemInteraction.Instance.PickupItem(gameObject);
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

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
