using UnityEngine;

public class FallingObjectController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float fallSpeed = 1.5f; // Reduced from 3f to 1.5f
    public bool straightFallingOnly = false; // New option for straight falling only
    
    [Header("Rotation Settings")]
    public float rotationSpeed = 50f;
    public Vector3 rotationAxis = Vector3.forward; // Default to z-axis rotation
    public bool randomizeRotationDirection = true;
    public bool randomizeRotationSpeed = true;
    [Range(0.5f, 2f)]
    public float rotationSpeedVariance = 1.5f;
    
    [Header("Boundaries")]
    public float bottomBoundary = -6f; // When to destroy the object
    
    // Private variables
    private float actualRotationSpeed;
    private Vector3 actualRotationAxis;
    private Vector3 initialPosition; // Store initial X and Z position
    private bool initialized = false;
    
    void Start()
    {
        // Store initial position (X and Z coordinates)
        initialPosition = new Vector3(transform.position.x, 0, transform.position.z);
        
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
        
        // Optional: Add a slight random initial rotation to make each object look different
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        
        // Log initial values for debugging
        Debug.Log($"[{gameObject.name}] FallingObjectController initialized with fallSpeed: {fallSpeed}, straightFallingOnly: {straightFallingOnly}");
        
        initialized = true;
    }
    
    void Update()
    {
        if (!initialized)
        {
            Start();
        }
        
        if (straightFallingOnly)
        {
            // For straight falling, maintain X and Z position and only change Y
            float newY = transform.position.y - (fallSpeed * Time.deltaTime);
            transform.position = new Vector3(initialPosition.x, newY, initialPosition.z);
            
            // Debug log movement occasionally
            if (Time.frameCount % 60 == 0) // Log once every 60 frames to avoid spam
            {
                Debug.Log($"[{gameObject.name}] Moving straight down with speed: {fallSpeed}, position: {transform.position}");
            }
        }
        else
        {
            // Original movement behavior
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
            
            // Debug log movement occasionally
            if (Time.frameCount % 60 == 0) // Log once every 60 frames to avoid spam
            {
                Debug.Log($"[{gameObject.name}] Moving with translate, speed: {fallSpeed}, position: {transform.position}");
            }
        }
        
        // Rotate the object (if rotation speed is not zero)
        if (actualRotationSpeed != 0)
        {
            transform.Rotate(actualRotationAxis * actualRotationSpeed * Time.deltaTime);
        }
        
        // Destroy if below screen
        if (transform.position.y < bottomBoundary)
        {
            Destroy(gameObject);
        }
    }
    
    // Add a method to manually set the fall speed at runtime
    public void SetFallSpeed(float newSpeed)
    {
        fallSpeed = newSpeed;
        Debug.Log($"[{gameObject.name}] Fall speed changed to: {fallSpeed}");
    }
}
