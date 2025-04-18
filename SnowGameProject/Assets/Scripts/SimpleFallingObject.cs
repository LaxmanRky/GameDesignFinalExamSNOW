using UnityEngine;

public class SimpleFallingObject : MonoBehaviour
{
    [Header("Movement")]
    [Range(0.05f, 5.0f)]
    public float fallSpeed = 0.5f; // Reduced default and added Range attribute
    
    [Tooltip("Multiplies the fall speed. Use this for fine-tuning (e.g., 0.1 for very slow, 1.0 for normal)")]
    [Range(0.1f, 2.0f)]
    public float speedMultiplier = 1.0f; // Add a multiplier for easier fine-tuning
    
    public bool straightFall = true;
    
    [Header("Rotation")]
    public bool rotate = true;
    public float rotationSpeed = 50f;
    
    [Header("Cleanup")]
    public float destroyBelowY = -10f;
    
    [Header("Collision")]
    public bool enableCollisions = true;
    
    private Vector3 initialXZ;
    private float actualSpeed; // The actual speed after applying the multiplier
    
    void Start()
    {
        // Calculate the actual speed
        actualSpeed = fallSpeed * speedMultiplier;
        
        // Store initial X and Z position
        initialXZ = new Vector3(transform.position.x, 0, transform.position.z);
        
        // Log for debugging
        Debug.Log($"[{gameObject.name}] SimpleFallingObject initialized with fallSpeed: {fallSpeed}, multiplier: {speedMultiplier}, actual speed: {actualSpeed}");
        
        // Ensure object has proper tag based on name
        if (gameObject.name.ToLower().Contains("snow") && gameObject.tag != "Snow")
        {
            gameObject.tag = "Snow";
            Debug.Log($"[{gameObject.name}] Set tag to Snow");
        }
        else if ((gameObject.name.ToLower().Contains("debris") || gameObject.name.ToLower().Contains("asteroid")) && gameObject.tag != "Debris")
        {
            gameObject.tag = "Debris";
            Debug.Log($"[{gameObject.name}] Set tag to Debris");
        }
        
        // Make sure the object has a collider
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null && enableCollisions)
        {
            BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
            boxCollider.isTrigger = true; // Use trigger for better collision detection
            Debug.Log($"[{gameObject.name}] Added BoxCollider2D with isTrigger=true");
        }
        else if (collider != null && enableCollisions)
        {
            // Make sure the collider is a trigger
            collider.isTrigger = true;
            Debug.Log($"[{gameObject.name}] Set existing collider to isTrigger=true");
        }
        
        // Disable any Rigidbody components that might interfere with our movement
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            Debug.Log($"[{gameObject.name}] Disabled Rigidbody gravity and set to kinematic");
        }
        
        Rigidbody2D rb2d = GetComponent<Rigidbody2D>();
        if (rb2d != null)
        {
            rb2d.gravityScale = 0;
            rb2d.bodyType = RigidbodyType2D.Kinematic;
            Debug.Log($"[{gameObject.name}] Disabled Rigidbody2D gravity and set to kinematic");
        }
        
        // Disable other movement controllers to avoid conflicts
        DisableOtherControllers();
    }
    
    void Update()
    {
        // Move the object down based on actual speed
        if (straightFall)
        {
            // Move straight down, maintaining X and Z position
            float newY = transform.position.y - (actualSpeed * Time.deltaTime);
            transform.position = new Vector3(initialXZ.x, newY, initialXZ.z);
        }
        else
        {
            // Move down using Translate
            transform.Translate(Vector3.down * actualSpeed * Time.deltaTime);
        }
        
        // Rotate the object if enabled
        if (rotate)
        {
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }
        
        // Destroy if below threshold
        if (transform.position.y < destroyBelowY)
        {
            Destroy(gameObject);
        }
    }
    
    private void DisableOtherControllers()
    {
        // Disable other controllers to avoid conflicts
        SnowController snowController = GetComponent<SnowController>();
        if (snowController != null && snowController != this)
        {
            snowController.enabled = false;
            Debug.Log($"[{gameObject.name}] Disabled SnowController");
        }
        
        FallingObjectController fallingController = GetComponent<FallingObjectController>();
        if (fallingController != null && fallingController != this)
        {
            fallingController.enabled = false;
            Debug.Log($"[{gameObject.name}] Disabled FallingObjectController");
        }
        
        DebrisController debrisController = GetComponent<DebrisController>();
        if (debrisController != null && debrisController != this)
        {
            debrisController.enabled = false;
            Debug.Log($"[{gameObject.name}] Disabled DebrisController");
        }
    }
    
    // Method to change the speed at runtime
    public void SetSpeed(float newSpeed, float newMultiplier = 1.0f)
    {
        fallSpeed = Mathf.Clamp(newSpeed, 0.05f, 5.0f);
        speedMultiplier = Mathf.Clamp(newMultiplier, 0.1f, 2.0f);
        actualSpeed = fallSpeed * speedMultiplier;
        
        Debug.Log($"[{gameObject.name}] Speed changed to: {fallSpeed} × {speedMultiplier} = {actualSpeed}");
    }
}
