using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DateReader : MonoBehaviour
{
    TMP_Text _dateText;

    private void Start()
    {
        _dateText = GetComponent<TMP_Text>();
    }
    void Update()
    {
        _dateText.text = "Day " + TimeManager.Instance.time.days;
    }
}
