using UnityEngine;

public class EnemyHealthUI : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private HealthBarView view;

    private void OnEnable()
    {
        health.OnHealthChanges += view.UpdateView;
    }

    private void OnDisable()
    {
        health.OnHealthChanges -= view.UpdateView;
    }
}
