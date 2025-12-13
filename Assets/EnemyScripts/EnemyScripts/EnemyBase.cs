using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(EnemyVision))]
public abstract class EnemyBase : MonoBehaviour
{
    public EnemyStatsSO stats;

    [HideInInspector] public float currentHealth;
    [HideInInspector] public EnemyMovement movement;
    [HideInInspector] public EnemyVision vision;
    [HideInInspector] public Animator animator;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<EnemyMovement>();
        vision = GetComponent<EnemyVision>();

        currentHealth = stats != null ? stats.maxHealth : 100f;

        if (EnemyManager.Instance != null)
            EnemyManager.Instance.Register(this);

        if (stats != null)
        {
            movement.moveSpeed = stats.moveSpeed;
            vision.visionRange = stats.visionRange;
        }
    }

    protected virtual void OnDestroy()
    {
        if (EnemyManager.Instance != null)
            EnemyManager.Instance.Unregister(this);
    }

    public virtual void TakeDamage(float dmg)
    {
        currentHealth -= dmg;
        if (animator != null)
            animator.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            if (animator != null)
                animator.SetTrigger("Death");
            Destroy(gameObject, 2f);
        }
    }
}