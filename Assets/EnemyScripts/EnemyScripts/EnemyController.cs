using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Sprite[] idleSprites;
    public Sprite[] walkSprites;
    public float frameRate = 0.15f;

    private SpriteAnimator animator;
    private EnemyMovement movement;
    private EnemyAttack attack;
    private EnemyJump jump;
    private EnemyDeath death;

    private bool isMoving;

    private void Awake()
    {
        animator = GetComponent<SpriteAnimator>();
        movement = GetComponent<EnemyMovement>();
        attack = GetComponent<EnemyAttack>();
        jump = GetComponent<EnemyJump>();
        death = GetComponent<EnemyDeath>();
    }

    public void Move(Vector2 dir)
    {
        isMoving = true;
        movement.Move(dir);
        animator.Play(walkSprites, frameRate, true);
    }

    public void Stop()
    {
        if (!isMoving) return;
        isMoving = false;

        movement.Stop();
        animator.Play(idleSprites, frameRate, true);
    }

    public void Attack()
    {
        attack.Attack();
    }

    public void Jump()
    {
        jump.Jump();
    }

    public void Die()
    {
        death.Die();
    }
}
