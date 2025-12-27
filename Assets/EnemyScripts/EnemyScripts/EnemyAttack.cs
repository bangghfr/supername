using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public Sprite[] attackSprites;
    public float frameRate = 0.1f;
    public float cooldown = 1f;

    private SpriteAnimator animator;
    private bool canAttack = true;

    private void Awake()
    {
        animator = GetComponent<SpriteAnimator>();
    }

    public void Attack()
    {
        if (!canAttack) return;
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        canAttack = false;
        animator.Play(attackSprites, frameRate, false);
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }
}
