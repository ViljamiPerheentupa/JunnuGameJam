using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IForceInheritance
{
    void InheritForce(Vector3 force, float acceleration);
    void AddJumpForce(float force);
}
public interface IChangeMovementData
{
    void ChangeMovementData(MovementData data);

    void RevertMovementData();
}

public interface IInteractable
{
    void Interact();
    string ObjectName();
    InteractType InteractionType();
}