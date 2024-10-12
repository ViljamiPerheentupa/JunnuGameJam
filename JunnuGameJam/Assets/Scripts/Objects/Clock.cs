using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Clock : MonoBehaviour
{
    TMP_Text _clockText;

    private void Start()
    {
        _clockText = GetComponent<TMP_Text>();
    }
    void Update()
    {
        _clockText.text = TimeManager.Instance.CurrentTime();
    }
}
