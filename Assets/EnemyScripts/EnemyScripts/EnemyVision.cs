using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    public float visionRange = 6f;
    public LayerMask playerLayer;

    public bool CanSeePlayer(out Transform player)
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, visionRange, playerLayer);
        if (hit != null)
        {
            player = hit.transform;
            return true;
        }
        player = null;
        return false;
    }
}