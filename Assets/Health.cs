using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private int _currentHealth;
    private int _HealthMax;

    private Action _OnDeath;
    private Action <int, int> _OnHealthChanges;
    private void Awake()
    {
        _currentHealth = _HealthMax;
    }

    private void Change(int value)
    {
        _currentHealth += value;
        if (_currentHealth > _HealthMax)
        {
            _currentHealth = _HealthMax;
        }
        if (_currentHealth <= 0)
        {
            _OnDeath?.Invoke();
        }
        _OnHealthChanges?.Invoke(_currentHealth, _HealthMax);
        //_OnDeath.Invoke;
    }
}
