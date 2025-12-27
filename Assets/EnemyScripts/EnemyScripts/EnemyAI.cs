using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    public float chaseDistance = 5f;
    public float attackDistance = 1.5f;

    private EnemyController controller;
    private EnemyVision vision;

    private void Awake()
    {
        controller = GetComponent<EnemyController>();
        vision = GetComponent<EnemyVision>();
    }

    private void Update()
    {
        if (target == null) return;
        if (!vision.CanSeeTarget(target))
        {
            controller.Stop();
            return;
        }

        float dist = Vector2.Distance(transform.position, target.position);
        Vector2 dir = (target.position - transform.position).normalized;

        if (dist < chaseDistance)
        {
            Debug.Log("dist < chaseDistance");
            controller.Stop();
        }
        else if (dist > attackDistance)
        {
            Debug.Log("dist > attackDistance");
            controller.Move(dir);
        }
        else
        {
            Debug.Log("else");
            controller.Stop();
            controller.Attack();
        }
    }
}
