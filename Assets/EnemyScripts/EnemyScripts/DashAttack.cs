using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class DashAttack : MonoBehaviour
{
    public float dashSpeed = 8f;
    public float dashDuration = 0.2f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public IEnumerator DashToward(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        rb.velocity = direction * dashSpeed;
        yield return new WaitForSeconds(dashDuration);
        rb.velocity = Vector2.zero;
    }
}