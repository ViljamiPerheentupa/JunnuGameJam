using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ThrowableObject : MonoBehaviour, IInteractable
{
    public string objectName;
    [SerializeField] private string prompt;
    [SerializeField] UnityEvent _onCollisionEvent;

    public InteractType interactType { get; private set; }

    private void OnCollisionEnter(Collision collision)
    {
        _onCollisionEvent.Invoke();
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
