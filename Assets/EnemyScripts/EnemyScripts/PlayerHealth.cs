using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100f;

    public void TakeDamage(float dmg)
    {
        health -= dmg;
        Debug.Log($"Player took {dmg} damage. Health left: {health}");
    }
}