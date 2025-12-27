using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyJump : MonoBehaviour
{
    public float jumpForce = 6f;
    public Sprite[] jumpSprites;
    public float frameRate = 0.12f;

    private Rigidbody2D rb;
    private SpriteAnimator animator;
    private bool canJump = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<SpriteAnimator>();
    }

    public void Jump()
    {
        if (!canJump) return;

        canJump = false;
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        if (jumpSprites != null && jumpSprites.Length > 0)
            animator.Play(jumpSprites, frameRate, false);

        Invoke(nameof(ResetJump), 0.5f);
    }

    private void ResetJump()
    {
        canJump = true;
    }
}
