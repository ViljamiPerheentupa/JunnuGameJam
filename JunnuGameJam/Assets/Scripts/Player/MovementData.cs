using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Goob/New Movement Data")]
public class MovementData : ScriptableObject
{
    public string dataName;

    public float movementSpeed;

    public float jumpForce;

    public float airControl;
    public float airAcceleration;
    public float airDeacceleration;

    public float sideStrafeSpeed;
    public float sideStrafeAcceleration;

    public float runAcceleration;
    public float runDeacceleration;

    public float friction;
}
