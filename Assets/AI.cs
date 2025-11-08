using System.Collections;
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
    float distance;



    void Update()
    {
        //Debug.Log(Enemy.transform.position.x - Player.transform.position.x <= 15);
        //Debug.Log(Enemy.transform.position.x - Player.transform.position.x >= -30);
        distance = Vector2.Distance(Player.position, Enemy.position);
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
        if (Enemy.transform.position.x > 15 && Enemy.transform.position.x > -15)
        {
            Idle();
        }    
    }

    private IEnumerator Delay()
    {
      
        yield return new WaitForSeconds(1);
    }


    void Idle()
    {
        float random = Random.Range(0, 5000);
        float SuperRandom = Random.Range(0, 10000);
        if (distance > 7)
        {
            if (random < 50)
            {
                Enemy.transform.Rotate(0, -180, 0);
                transform.position += new Vector3(-0.01f, 0, 0);
            }
            if (SuperRandom < 2)
            {
                Enemy.transform.Rotate(0, 0, 0);
                transform.position += new Vector3(0.01f, 0, 0);
            }
        }

        if (distance < 7)
        {
            if (random < 50)
            {
                Enemy.transform.Rotate(0, 0, 0);
                transform.position += new Vector3(0.01f, 0, 0);
            }
            if (SuperRandom < 2)
            {
                Enemy.transform.Rotate(0, -180, 0);
                transform.position += new Vector3(-0.01f, 0, 0);
            }
        }
    }

    void Shoot()
    {
        // Определяем направление стрельбы по горизонтальному движению игрока
        // Если Move.x == 0 — можно стрелять вправо по умолчанию (или хранить последнее направление)
      

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

        if (distance < -7)
        {
            rb.AddForce(-Enemy.transform.right * bulletSpeed);
            StartCoroutine(Delay());
        }
        else if (distance > 7) 
        {
            rb.AddForce(Enemy.transform.right * bulletSpeed);
            StartCoroutine(Delay());
        }
    }
}
