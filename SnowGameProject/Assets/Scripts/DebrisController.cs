using UnityEngine;

public class DebrisController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float fallSpeed = 4f;
    public bool straightFallingOnly = false; // Added to match FallingObjectController
    
    [Header("Rotation Settings")]
    public float rotationSpeed = 70f;
    public Vector3 rotationAxis = Vector3.forward; // Default to z-axis rotation
    public bool randomizeRotationDirection = true;
    public bool randomizeRotationSpeed = true;
    [Range(0.5f, 2f)]
    public float rotationSpeedVariance = 1.5f;
    
    [Header("Boundaries")]
    public float bottomBoundary = -6f; // When to destroy the debris object
    
    // Private variables
    private float actualRotationSpeed;
    private Vector3 actualRotationAxis;
    private Vector3 initialPosition; // Store initial X and Z position
    private FallingObjectController fallingController; // Reference to FallingObjectController if present
    
    void Start()
    {
        // Ensure the debris has the correct tag
        gameObject.tag = "Debris";
        
        // Store initial position (X and Z coordinates) for straight falling
        initialPosition = new Vector3(transform.position.x, 0, transform.position.z);
        
        // Check if there's a FallingObjectController on this object
        fallingController = GetComponent<FallingObjectController>();
        if (fallingController != null)
        {
            // If FallingObjectController exists, copy its fall speed to this controller
            fallSpeed = fallingController.fallSpeed;
            straightFallingOnly = fallingController.straightFallingOnly;
            
            // Log for debugging
            Debug.Log($"[{gameObject.name}] DebrisController found FallingObjectController with fallSpeed: {fallingController.fallSpeed}");
        }
        
        // Randomize rotation direction if enabled
        if (randomizeRotationDirection)
        {
            // 50% chance to reverse the rotation direction
            if (Random.value > 0.5f)
            {
                rotationAxis = -rotationAxis;
            }
        }
        
        // Randomize rotation speed if enabled
        if (randomizeRotationSpeed)
        {
            // Vary the rotation speed by the specified variance
            actualRotationSpeed = rotationSpeed * Random.Range(1f / rotationSpeedVariance, rotationSpeedVariance);
        }
        else
        {
            actualRotationSpeed = rotationSpeed;
        }
        
        // Store the actual rotation axis
        actualRotationAxis = rotationAxis;
        
        // Optional: Add a slight random initial rotation to make each debris look different
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
    }
    
    void Update()
    {
        // If there's a FallingObjectController, let it handle movement
        if (fallingController != null)
        {
            // Just handle rotation in this script
            transform.Rotate(actualRotationAxis * actualRotationSpeed * Time.deltaTime);
            
            // Check for destruction below screen
            if (transform.position.y < bottomBoundary)
            {
                Destroy(gameObject);
            }
            
            // Skip the rest of the update since FallingObjectController will handle movement
            return;
        }
        
        // If we get here, there's no FallingObjectController, so handle movement ourselves
        if (straightFallingOnly)
        {
            // For straight falling, maintain X and Z position and only change Y
            float newY = transform.position.y - (fallSpeed * Time.deltaTime);
            transform.position = new Vector3(initialPosition.x, newY, initialPosition.z);
        }
        else
        {
            // Original movement behavior
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
        }
        
        // Rotate the debris
        transform.Rotate(actualRotationAxis * actualRotationSpeed * Time.deltaTime);
        
        // Destroy if below screen
        if (transform.position.y < bottomBoundary)
        {
            Destroy(gameObject);
        }
    }
}
