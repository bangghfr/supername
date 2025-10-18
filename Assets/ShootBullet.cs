using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(InputReceiver))]
public class ShootController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;

    private InputReceiver inputReceiver;

    void Start()
    {
        inputReceiver = GetComponent<InputReceiver>();
        if (inputReceiver == null)
        {
            Debug.LogError("InputReceiver component not found on the player!");
        }
    }

    void Update()
    {
        if (inputReceiver != null && inputReceiver.AtackPressed)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Определяем направление стрельбы по горизонтальному движению игрока
        // Если Move.x == 0 — можно стрелять вправо по умолчанию (или хранить последнее направление)
        float horizontal = inputReceiver.Move.x;
        bool shootRight = horizontal >= 0;

        // Создаём пулю
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);


        Vector2 direction = shootRight ? Vector2.right : Vector2.left;

        // Поворачиваем пулю, чтоб смотрела в нужную сторону
        float angle = shootRight ? 0f : 180f;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * bulletSpeed;
        }
    }
}
