using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    public float viewDistance = 6f;
    public LayerMask targetLayer;
    public LayerMask obstacleLayer;

    public bool CanSeeTarget(Transform target)
    {
        if (target == null) return false;

        float dist = Vector2.Distance(transform.position, target.position);
        if (dist > viewDistance) return false;

        RaycastHit2D hit = Physics2D.Linecast(
            transform.position,
            target.position,
            obstacleLayer | targetLayer
        );

        if (hit.collider == null) return false;

        return ((1 << hit.collider.gameObject.layer) & targetLayer) != 0;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
    }
#endif
}
