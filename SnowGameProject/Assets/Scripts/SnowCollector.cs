using UnityEngine;

public class SnowCollector : MonoBehaviour
{
    private GameManager gameManager;
    
    void Start()
    {
        // Find the GameManager in the scene
        gameManager = FindAnyObjectByType<GameManager>();
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is snow
        if (other.CompareTag("Snow"))
        {
            // Add points to the score
            if (gameManager != null)
            {
                gameManager.AddPoints(5);
            }
            
            // Destroy the snow object
            Destroy(other.gameObject);
        }
    }
}
