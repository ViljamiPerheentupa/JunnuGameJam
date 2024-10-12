using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableObject : MonoBehaviour, IInteractable
{
    public string objectName;
    [SerializeField] private string prompt;

    public InteractType interactType { get; private set; }

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
}
