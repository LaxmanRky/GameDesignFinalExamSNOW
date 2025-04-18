using UnityEngine;

public class AstronautCollector : MonoBehaviour
{
    private GameManager gameManager;
    
    [Header("Debug")]
    public bool showDebugMessages = true;
    
    void Start()
    {
        // Find the GameManager
        gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            // Try to find using FindObjectOfType
            gameManager = FindObjectOfType<GameManager>();
            
            if (gameManager == null)
            {
                // Last resort: try to find by name
                GameObject gmObj = GameObject.Find("GameManager");
                if (gmObj != null)
                {
                    gameManager = gmObj.GetComponent<GameManager>();
                }
            }
        }
        
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found! Make sure it exists in the scene.");
        }
        else
        {
            Debug.Log("AstronautCollector successfully found GameManager: " + gameManager.name);
        }
        
        // Make sure the astronaut has a collider
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            Debug.LogWarning("Astronaut has no Collider2D! Adding one automatically.");
            BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
            boxCollider.size = new Vector2(1f, 1.8f);
        }
        
        // Enable trigger mode for the collider
        if (collider != null && !collider.isTrigger)
        {
            Debug.Log("Setting astronaut collider to trigger mode for better collision detection");
            collider.isTrigger = true;
        }
        
        // Log all active colliders on this object
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            Debug.Log($"Astronaut has collider: {col.GetType().Name}, isTrigger: {col.isTrigger}");
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (showDebugMessages) Debug.Log($"Trigger collision with {other.gameObject.name}, tag: {other.gameObject.tag}");
        HandleCollision(other.gameObject);
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (showDebugMessages) Debug.Log($"Physical collision with {collision.gameObject.name}, tag: {collision.gameObject.tag}");
        HandleCollision(collision.gameObject);
    }
    
    void HandleCollision(GameObject other)
    {
        string tag = other.tag.ToLower(); // Convert to lowercase for case-insensitive comparison
        
        // Check if we hit snow
        if (tag == "snow")
        {
            if (showDebugMessages) Debug.Log($"Collected snow! {other.name}");
            
            // Add water to the water level
            if (gameManager != null)
            {
                gameManager.AddWater(gameManager.snowWaterIncrease);
                
                // Play collection sound or effect if you have one
                // AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }
            
            // Destroy the snow object
            Destroy(other.gameObject);
        }
        // Check if we hit debris
        else if (tag == "debris")
        {
            if (showDebugMessages) Debug.Log($"Hit by debris! {other.name}");
            
            // Take damage
            if (gameManager != null)
            {
                gameManager.TakeDamage(gameManager.debrisDamage);
                
                // Play damage sound or effect if you have one
                // AudioSource.PlayClipAtPoint(damageSound, transform.position);
                
                // Optional: Flash the player red to indicate damage
                StartCoroutine(FlashRed());
            }
            
            // Destroy the debris object
            Destroy(other.gameObject);
        }
        else
        {
            if (showDebugMessages) Debug.Log($"Collision with unhandled tag: '{tag}' on object {other.name}");
        }
    }
    
    System.Collections.IEnumerator FlashRed()
    {
        // Get the sprite renderer
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            // Store original color
            Color originalColor = renderer.color;
            
            // Change to red
            renderer.color = Color.red;
            
            // Wait a short time
            yield return new WaitForSeconds(0.1f);
            
            // Change back to original color
            renderer.color = originalColor;
        }
    }
}
