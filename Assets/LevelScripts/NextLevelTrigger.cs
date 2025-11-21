using System.Collections;
using UnityEngine;

public class NextLevelTrigger : MonoBehaviour
{
    public CaveGenerator2D generator;  // ссылка на генератор
    public Transform player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            generator.GenerateNextLevel(player);
        }
    }
}
