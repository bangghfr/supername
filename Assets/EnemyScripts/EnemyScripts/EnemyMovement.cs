using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    public float speed = 2f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    public void Move(Vector2 dir)
    {
        rb.velocity = dir.normalized * speed;

        // поворот спрайта
        if (dir.x != 0)
            sr.flipX = dir.x < 0;
    }

    public void Stop()
    {
        rb.velocity = Vector2.zero;
    }
}
