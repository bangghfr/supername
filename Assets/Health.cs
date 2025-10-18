using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private int _currentHealth;
    private int _healthMax = 100;
    private int timer = 0;

    public Action OnDeath;
    public Action <int, int> OnHealthChanges;
    private void Awake()
    {
        _currentHealth = _healthMax;
        
    }

    private void Change()
    {
        _currentHealth -= AI.value;
        if (timer == 2)
        {
            StartCoroutine(Wait());
        }
        if (_currentHealth > _healthMax)
        {
            _currentHealth = _healthMax;
        }
        if (_currentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
        OnHealthChanges?.Invoke(_currentHealth, _healthMax);
        //_OnDeath.Invoke;
    }

    IEnumerator Wait()
    {
        while (true)
        {
            if (_currentHealth < 85)
            {
                Debug.Log("TimerCount: " + (timer++));
                yield return new WaitForSeconds(1);
            }

            yield return null;
        }
    }

    void Start()
    {
        StartCoroutine(Wait());
    }
}

