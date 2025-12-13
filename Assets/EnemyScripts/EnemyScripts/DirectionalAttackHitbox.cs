using UnityEngine;

public class DirectionalAttackHitbox : MonoBehaviour
{
    public float damage = 10f;
    public LayerMask targetMask;
    public float lifetime = 0.15f;
    public Vector2 direction = Vector2.right;
    public float distance = 1f;

    private void Start()
    {
        // Повернуть хитбокс в нужную сторону
        transform.right = direction;

        // Сместить в сторону атаки
        transform.position += (Vector3)(direction.normalized * distance);

        // Удалить через время жизни
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Получить PlayerHealth (или заменить на свой скрипт)
        var hp = other.GetComponent<PlayerHealth>();

        if (hp != null)
        {
            hp.TakeDamage(damage);
        }
    }
}
