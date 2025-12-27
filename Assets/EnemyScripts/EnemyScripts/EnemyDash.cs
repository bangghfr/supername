using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyDash : MonoBehaviour
{
    public float force = 6f;
    public float cooldown = 2f;

    private Rigidbody2D rb;
    private bool canDash = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Dash(Vector2 dir)
    {
        if (!canDash) return;
        StartCoroutine(DashRoutine(dir));
    }

    private IEnumerator DashRoutine(Vector2 dir)
    {
        canDash = false;
        rb.AddForce(dir.normalized * force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(cooldown);
        canDash = true;
    }
}
