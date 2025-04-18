using UnityEngine;

public class FallingObjectSpawnPoint : MonoBehaviour
{
    // This script should be attached to each spawn point
    
    [Header("Falling Settings")]
    public float defaultFallSpeed = 1.5f; // Controls the default fall speed for objects spawned from this point
    public bool randomizeFallSpeed = false;
    public float minFallSpeed = 1f;
    public float maxFallSpeed = 2.5f;
    
    [Header("Debug")]
    public bool showDebugInfo = true;
    
    // Reference to the main spawner that contains the prefabs
    private SnowDebrisSpawner spawner;
    
    private void Start()
    {
        // Find the spawner in the scene
        spawner = FindAnyObjectByType<SnowDebrisSpawner>();
        
        if (spawner == null)
        {
            Debug.LogError($"SnowDebrisSpawner not found in the scene! Spawn point {gameObject.name} will not work.");
        }
    }
    
    // This method will be called by Timeline signals
    public void SpawnObject(int prefabIndex)
    {
        if (spawner != null && spawner.fallingObjectPrefabs.Count > prefabIndex && prefabIndex >= 0)
        {
            // Determine fall speed
            float fallSpeed = defaultFallSpeed;
            if (randomizeFallSpeed)
            {
                fallSpeed = Random.Range(minFallSpeed, maxFallSpeed);
            }
            
            // Spawn the specified prefab at this spawn point's position
            GameObject spawnedObject = Instantiate(spawner.fallingObjectPrefabs[prefabIndex], transform.position, Quaternion.identity);
            
            // Apply the fall speed to the spawned object
            ApplyFallSpeed(spawnedObject, fallSpeed);
        }
        else
        {
            Debug.LogWarning($"Cannot spawn object with index {prefabIndex}. Check if the prefab exists in the SnowDebrisSpawner.");
        }
    }
    
    // Method to spawn with specific speed
    public void SpawnObjectWithSpeed(int prefabIndex, float speed)
    {
        if (spawner != null && spawner.fallingObjectPrefabs.Count > prefabIndex && prefabIndex >= 0)
        {
            // Spawn the specified prefab at this spawn point's position
            GameObject spawnedObject = Instantiate(spawner.fallingObjectPrefabs[prefabIndex], transform.position, Quaternion.identity);
            
            // Apply the specified fall speed to the spawned object
            ApplyFallSpeed(spawnedObject, speed);
        }
        else
        {
            Debug.LogWarning($"Cannot spawn object with index {prefabIndex}. Check if the prefab exists in the SnowDebrisSpawner.");
        }
    }
    
    // Helper method to apply fall speed to any type of controller
    private void ApplyFallSpeed(GameObject obj, float speed)
    {
        bool speedApplied = false;
        
        // Try to set speed on SnowController
        SnowController snowController = obj.GetComponent<SnowController>();
        if (snowController != null)
        {
            float oldSpeed = snowController.fallSpeed;
            snowController.fallSpeed = speed;
            snowController.straightFallingOnly = true;
            speedApplied = true;
            
            if (showDebugInfo)
            {
                Debug.Log($"[{obj.name}] Set SnowController speed from {oldSpeed} to {speed}");
            }
        }
        
        // Try to set speed on FallingObjectController
        FallingObjectController fallingController = obj.GetComponent<FallingObjectController>();
        if (fallingController != null)
        {
            float oldSpeed = fallingController.fallSpeed;
            fallingController.fallSpeed = speed;
            fallingController.straightFallingOnly = true;
            speedApplied = true;
            
            if (showDebugInfo)
            {
                Debug.Log($"[{obj.name}] Set FallingObjectController speed from {oldSpeed} to {speed}");
            }
        }
        
        // Try to set speed on DebrisController
        DebrisController debrisController = obj.GetComponent<DebrisController>();
        if (debrisController != null)
        {
            float oldSpeed = debrisController.fallSpeed;
            debrisController.fallSpeed = speed;
            debrisController.straightFallingOnly = true;
            speedApplied = true;
            
            if (showDebugInfo)
            {
                Debug.Log($"[{obj.name}] Set DebrisController speed from {oldSpeed} to {speed}");
            }
        }
        
        // If no controller was found, add a FallingObjectController
        if (!speedApplied)
        {
            fallingController = obj.AddComponent<FallingObjectController>();
            fallingController.fallSpeed = speed;
            fallingController.straightFallingOnly = true;
            
            if (showDebugInfo)
            {
                Debug.Log($"[{obj.name}] Added FallingObjectController with speed {speed}");
            }
        }
    }
}
