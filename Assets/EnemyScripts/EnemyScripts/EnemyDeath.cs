using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    public Sprite[] deathSprites;
    public float frameRate = 0.12f;

    private SpriteAnimator animator;
    private bool dead;

    private void Awake()
    {
        animator = GetComponent<SpriteAnimator>();
    }

    public void Die()
    {
        if (dead) return;
        dead = true;

        animator.Play(deathSprites, frameRate, false);

        // отключаем логику
        GetComponent<EnemyAI>().enabled = false;
        GetComponent<EnemyMovement>().enabled = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        Destroy(gameObject, deathSprites.Length * frameRate + 0.2f);
    }
}
