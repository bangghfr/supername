using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int _healthMax = 100;
    private int _currentHealth;

    public Action OnDeath;
    public Action<int, int> OnHealthChanges;

    private void Awake()
    {
        _currentHealth = _healthMax;
        OnHealthChanges?.Invoke(_currentHealth, _healthMax);
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _healthMax);

        OnHealthChanges?.Invoke(_currentHealth, _healthMax);

        if (_currentHealth <= 0)
            OnDeath?.Invoke();
    }
}
