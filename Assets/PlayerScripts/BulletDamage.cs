using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDamage : MonoBehaviour
{
    public int damage = 25;
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime); // Автоуничтожение через время
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            SoundManager.Instance.PlaySound(SoundManager.SoundType.Attack);
            Destroy(gameObject); // Уничтожаем пулю после удара
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            // Если пуля попала в землю, тоже уничтожаем
            Destroy(gameObject);
        }
    }
}


