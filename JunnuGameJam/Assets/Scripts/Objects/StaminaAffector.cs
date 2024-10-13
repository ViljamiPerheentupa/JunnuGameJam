using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaAffector : MonoBehaviour
{
    [SerializeField] float _staminaPerUse = 75f;
    public void AddStamina()
    {
        Stamina.Instance.AddStamina(_staminaPerUse);
    }
}
