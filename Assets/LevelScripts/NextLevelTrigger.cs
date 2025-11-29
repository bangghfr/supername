using UnityEngine;

public class NextLevelTrigger : MonoBehaviour
{
    public CavesGenerator2D generator;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (generator == null)
                generator = FindObjectOfType<CavesGenerator2D>();

            //generator.GenerateNextLevel();
        }
    }
}
