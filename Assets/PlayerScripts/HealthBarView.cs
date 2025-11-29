using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarView : MonoBehaviour
{
    [SerializeField] private Image _image;
    public void UpdateView(int currentHealth, int maxHealth)
    {
        _image.fillAmount = currentHealth / maxHealth;
    }

}
