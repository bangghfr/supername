using Unity.VisualScripting;
using UnityEngine;

public class AI : MonoBehaviour
{
    static public int value = 15;

    public GameObject bulletPrefab;
    public Transform Enemy;
    public Transform firePoint;
    public float bulletSpeed = 70f;
    private bool check = false;
    public Transform Player;



    void Update()
    {
        //Debug.Log(Enemy.transform.position.x - Player.transform.position.x <= 15);
        //Debug.Log(Enemy.transform.position.x - Player.transform.position.x >= -30);
        float distance = Vector2.Distance(Player.position, Enemy.position);
        Player = transform.Find("Player");
        //if (Enemy.transform.position.x > Player.transform.position.x)
        //{
        //    if (Enemy.transform.position.x - Player.transform.position.x <= 15 || Enemy.transform.position.x - Player.transform.position.x >= -30)
        //    {
        //        check = true;
        //    }d
        //}
        //if (Enemy.transform.position.x < Player.transform.position.x)
        //{
        //    if ( Player.transform.position.x - Enemy.transform.position.x <= 15 || Player.transform.position.x - Enemy.transform.position.x >= -30)
        //    {
        //        check = true;
        //    }
        //}
        if (distance < 15 || distance > -15)
        {
            check = true;
        }


        if (check == true)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Определяем направление стрельбы по горизонтальному движению игрока
        // Если Move.x == 0 — можно стрелять вправо по умолчанию (или хранить последнее направление)
        if (Enemy.transform.position.x > 15)
        {
            Enemy.transform.Rotate(0, -180, 0);
        }

        if (Enemy.transform.position.x < 15)
        {
            Enemy.transform.Rotate(0, 0, 0);
        }

        ////float horizontal = Enemy.transform.Move.x;
        bool shootLeft = Enemy.transform.position.x < 0;

        // Создаём пулю
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        Vector2 direction = shootLeft ? Vector2.left : Vector2.right;

        // Поворачиваем пулю, чтоб смотрела в нужную сторону
        float angle = shootLeft ? 0f : 180f;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * bulletSpeed;
        }

        Player = transform.Find("Player");
        //if (Enemy.transform.position.x > Player.transform.position.x)
        //{
        //    if (Enemy.transform.position.x - Player.transform.position.x > 15)
        //    {
        //        rb.AddForce(Enemy.transform.right * bulletSpeed);

        //    }
        //}
        //if (Enemy.transform.position.x < Player.transform.position.x)
        //{
        //    if (Player.transform.position.x - Enemy.transform.position.x > 15)
        //    {
        //        rb.AddForce(Enemy.transform.right * bulletSpeed);

        //    }
        //}
        //if (Enemy.transform.position.x > Player.transform.position.x)
        //{
        //    if (Enemy.transform.position.x - Player.transform.position.x < -30)
        //    {
        //        rb.AddForce(-Enemy.transform.right * bulletSpeed);

        //    }
        //}
        //if (Enemy.transform.position.x < Player.transform.position.x)
        //{
        //    if (Player.transform.position.x - Enemy.transform.position.x < -30)
        //    {
        //        rb.AddForce(-Enemy.transform.right * bulletSpeed);

        //    }
        //}

        float distance = Vector3.Distance(Player.position, Enemy.position);

        if (distance < -15)
        {
            rb.AddForce(-Enemy.transform.right * bulletSpeed);
        }
        else if (distance > 15) 
        {
            rb.AddForce(Enemy.transform.right * bulletSpeed);
        }
    }
}
