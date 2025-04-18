using UnityEngine;

public class BoundaryCollider : MonoBehaviour
{
    [Tooltip("Set to true to make this a left boundary, false for right boundary")]
    public bool isLeftBoundary = false;
    
    void Start()
    {
        // Make sure we have a collider
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider2D>();
            Debug.Log($"Added BoxCollider2D to {gameObject.name}");
        }
        
        // Make sure the collider is not a trigger
        boxCollider.isTrigger = false;
        
        // Make the collider slightly thicker for better collision detection
        Vector2 size = boxCollider.size;
        size.x = Mathf.Max(size.x, 0.5f);
        boxCollider.size = size;
        
        // Add a rigidbody to ensure proper collision detection
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            Debug.Log($"Added Rigidbody2D to {gameObject.name}");
        }
        
        // Configure the rigidbody
        rb.bodyType = RigidbodyType2D.Static;
        rb.simulated = true;
        
        // Log the setup
        Debug.Log($"Boundary {gameObject.name} initialized. Layer: {gameObject.layer}, Position: {transform.position}");
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if we collided with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"Player collided with boundary {gameObject.name}");
            
            // Get the player's controller
            AstronautController playerController = collision.gameObject.GetComponent<AstronautController>();
            if (playerController != null)
            {
                // Force the player to stop
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.linearVelocity = new Vector2(0, playerRb.linearVelocity.y);
                }
                
                // Push the player away slightly
                Vector3 pushDirection = isLeftBoundary ? Vector3.right : Vector3.left;
                collision.gameObject.transform.position += pushDirection * 0.1f;
            }
        }
    }
}
