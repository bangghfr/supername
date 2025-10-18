using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private TMP_Text _healthText;

    private void ChangeText (int value)
    {
        _healthText.text = value.ToString();
    }
}
