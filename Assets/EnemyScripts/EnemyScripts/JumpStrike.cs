using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class JumpStrike : MonoBehaviour
{
    public float jumpForce = 12f;
    public float strikeRadius = 1f;
    public LayerMask playerLayer;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public IEnumerator DoJumpStrike(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized + Vector2.up;
        rb.AddForce(direction * jumpForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.5f);

        Collider2D hit = Physics2D.OverlapCircle(transform.position, strikeRadius, playerLayer);
        if (hit != null)
        {
            hit.GetComponent<PlayerHealth>()?.TakeDamage(20);
        }
    }
}