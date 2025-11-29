using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthViewController : MonoBehaviour
{
    [SerializeField] private HealthBarView _healtView;
    [SerializeField] private Health _healtModel;

    private void Awake()
    {
        _healtModel.OnHealthChanges += ChangeHealth;
    }

    private void ChangeHealth(int currentHealth, int maxHealth)
    { 
        _healtView.UpdateView(currentHealth, maxHealth);
    }
}
