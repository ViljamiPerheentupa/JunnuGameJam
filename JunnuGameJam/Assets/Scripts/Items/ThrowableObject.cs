using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableObject : MonoBehaviour, IInteractable
{
    [SerializeField] string objectName;
    public void Interact()
    {
        Debug.Log(objectName);
    }
}
