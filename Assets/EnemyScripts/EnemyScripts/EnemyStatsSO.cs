using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Stats")]
public class EnemyStatsSO : ScriptableObject
{
    public float maxHealth = 50f;
    public float moveSpeed = 2f;
    public float attackDamage = 10f;
    public float visionRange = 5f;
}