using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 100;

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        Debug.Log("Враг получил урон: " + damageAmount);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Враг погиб");
        Destroy(gameObject);
    }
}
