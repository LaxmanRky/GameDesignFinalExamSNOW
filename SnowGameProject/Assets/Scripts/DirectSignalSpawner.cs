using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;

public class DirectSignalSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public List<GameObject> prefabs = new List<GameObject>();
    
    [Header("Spawn Points")]
    public List<Transform> spawnPoints = new List<Transform>();
    
    [Header("Speed Settings")]
    public float defaultFallSpeed = 1.5f;
    public bool useRandomSpeeds = false;
    public float minFallSpeed = 1f;
    public float maxFallSpeed = 2.5f;
    
    // Global signal receiver setup
    private void OnEnable()
    {
        // Auto-find spawn points if needed
        if (spawnPoints.Count == 0)
        {
            Transform spawnPointsParent = GameObject.Find("SpawnPoints")?.transform;
            if (spawnPointsParent != null)
            {
                foreach (Transform child in spawnPointsParent)
                {
                    spawnPoints.Add(child);
                }
                Debug.Log($"Found {spawnPoints.Count} spawn points automatically.");
            }
        }
    }
    
    // Methods that can be called directly from Timeline signals
    
    // Spawn at track 0 (first spawn point)
    public void SpawnAtTrack0(int prefabIndex = 0)
    {
        SpawnPrefabAtPoint(prefabIndex, 0);
    }
    
    // Spawn at track 1 (second spawn point)
    public void SpawnAtTrack1(int prefabIndex = 0)
    {
        SpawnPrefabAtPoint(prefabIndex, 1);
    }
    
    // Spawn at track 2 (third spawn point)
    public void SpawnAtTrack2(int prefabIndex = 0)
    {
        SpawnPrefabAtPoint(prefabIndex, 2);
    }
    
    // Spawn at track 3 (fourth spawn point)
    public void SpawnAtTrack3(int prefabIndex = 0)
    {
        SpawnPrefabAtPoint(prefabIndex, 3);
    }
    
    // Spawn with specific speed
    public void SpawnWithSpeed(int prefabIndex, int spawnPointIndex, float speed)
    {
        // Validate indices
        if (prefabIndex < 0 || prefabIndex >= prefabs.Count)
        {
            Debug.LogWarning($"Invalid prefab index: {prefabIndex}. Using default prefab.");
            prefabIndex = 0;
        }
        
        if (spawnPointIndex < 0 || spawnPointIndex >= spawnPoints.Count)
        {
            Debug.LogWarning($"Invalid spawn point index: {spawnPointIndex}. Using random spawn point.");
            spawnPointIndex = Random.Range(0, spawnPoints.Count);
        }
        
        if (prefabs.Count == 0)
        {
            Debug.LogError("No prefabs assigned to DirectSignalSpawner!");
            return;
        }
        
        if (spawnPoints.Count == 0)
        {
            Debug.LogError("No spawn points assigned to DirectSignalSpawner!");
            return;
        }
        
        // Get the spawn point component
        FallingObjectSpawnPoint spawnPoint = spawnPoints[spawnPointIndex].GetComponent<FallingObjectSpawnPoint>();
        
        // If the spawn point has the FallingObjectSpawnPoint component, use its SpawnObjectWithSpeed method
        if (spawnPoint != null)
        {
            spawnPoint.SpawnObjectWithSpeed(prefabIndex, speed);
        }
        else
        {
            // Fallback to regular instantiation
            GameObject spawnedObject = Instantiate(prefabs[prefabIndex], spawnPoints[spawnPointIndex].position, Quaternion.identity);
            
            // Try to set the fall speed on the spawned object
            SnowController snowController = spawnedObject.GetComponent<SnowController>();
            if (snowController != null)
            {
                snowController.fallSpeed = speed;
                snowController.straightFallingOnly = true;
            }
            
            FallingObjectController fallingController = spawnedObject.GetComponent<FallingObjectController>();
            if (fallingController != null)
            {
                fallingController.fallSpeed = speed;
                fallingController.straightFallingOnly = true;
            }
        }
    }
    
    // Spawn a specific prefab at a specific spawn point
    public void SpawnPrefabAtPoint(int prefabIndex, int spawnPointIndex)
    {
        // Validate indices
        if (prefabIndex < 0 || prefabIndex >= prefabs.Count)
        {
            Debug.LogWarning($"Invalid prefab index: {prefabIndex}. Using default prefab.");
            prefabIndex = 0;
        }
        
        if (spawnPointIndex < 0 || spawnPointIndex >= spawnPoints.Count)
        {
            Debug.LogWarning($"Invalid spawn point index: {spawnPointIndex}. Using random spawn point.");
            spawnPointIndex = Random.Range(0, spawnPoints.Count);
        }
        
        if (prefabs.Count == 0)
        {
            Debug.LogError("No prefabs assigned to DirectSignalSpawner!");
            return;
        }
        
        if (spawnPoints.Count == 0)
        {
            Debug.LogError("No spawn points assigned to DirectSignalSpawner!");
            return;
        }
        
        // Determine fall speed
        float fallSpeed = defaultFallSpeed;
        if (useRandomSpeeds)
        {
            fallSpeed = Random.Range(minFallSpeed, maxFallSpeed);
        }
        
        // Get the spawn point component
        FallingObjectSpawnPoint spawnPoint = spawnPoints[spawnPointIndex].GetComponent<FallingObjectSpawnPoint>();
        
        // If the spawn point has the FallingObjectSpawnPoint component, use its SpawnObjectWithSpeed method
        if (spawnPoint != null)
        {
            spawnPoint.SpawnObjectWithSpeed(prefabIndex, fallSpeed);
        }
        else
        {
            // Fallback to regular instantiation
            GameObject spawnedObject = Instantiate(prefabs[prefabIndex], spawnPoints[spawnPointIndex].position, Quaternion.identity);
            
            // Try to set the fall speed on the spawned object
            SnowController snowController = spawnedObject.GetComponent<SnowController>();
            if (snowController != null)
            {
                snowController.fallSpeed = fallSpeed;
                snowController.straightFallingOnly = true;
            }
            
            FallingObjectController fallingController = spawnedObject.GetComponent<FallingObjectController>();
            if (fallingController != null)
            {
                fallingController.fallSpeed = fallSpeed;
                fallingController.straightFallingOnly = true;
            }
        }
    }
    
    // Spawn a random prefab at a random spawn point
    public void SpawnRandom()
    {
        if (prefabs.Count == 0 || spawnPoints.Count == 0)
        {
            Debug.LogError("Missing prefabs or spawn points in DirectSignalSpawner!");
            return;
        }
        
        int randomPrefab = Random.Range(0, prefabs.Count);
        int randomSpawnPoint = Random.Range(0, spawnPoints.Count);
        
        // Determine fall speed
        float fallSpeed = defaultFallSpeed;
        if (useRandomSpeeds)
        {
            fallSpeed = Random.Range(minFallSpeed, maxFallSpeed);
        }
        
        // Get the spawn point component
        FallingObjectSpawnPoint spawnPoint = spawnPoints[randomSpawnPoint].GetComponent<FallingObjectSpawnPoint>();
        
        // If the spawn point has the FallingObjectSpawnPoint component, use its SpawnObjectWithSpeed method
        if (spawnPoint != null)
        {
            spawnPoint.SpawnObjectWithSpeed(randomPrefab, fallSpeed);
        }
        else
        {
            // Fallback to regular instantiation
            GameObject spawnedObject = Instantiate(prefabs[randomPrefab], spawnPoints[randomSpawnPoint].position, Quaternion.identity);
            
            // Try to set the fall speed on the spawned object
            SnowController snowController = spawnedObject.GetComponent<SnowController>();
            if (snowController != null)
            {
                snowController.fallSpeed = fallSpeed;
                snowController.straightFallingOnly = true;
            }
            
            FallingObjectController fallingController = spawnedObject.GetComponent<FallingObjectController>();
            if (fallingController != null)
            {
                fallingController.fallSpeed = fallSpeed;
                fallingController.straightFallingOnly = true;
            }
        }
    }
    
    // Spawn prefab 0 (typically snow)
    public void SpawnSnow(int spawnPointIndex = -1)
    {
        if (spawnPointIndex == -1)
        {
            spawnPointIndex = Random.Range(0, spawnPoints.Count);
        }
        
        SpawnPrefabAtPoint(0, spawnPointIndex);
    }
    
    // Spawn prefab 1 (typically debris)
    public void SpawnDebris(int spawnPointIndex = -1)
    {
        if (spawnPointIndex == -1)
        {
            spawnPointIndex = Random.Range(0, spawnPoints.Count);
        }
        
        SpawnPrefabAtPoint(1, spawnPointIndex);
    }
    
    // Spawn snow with specific speed
    public void SpawnSnowWithSpeed(int spawnPointIndex, float speed)
    {
        if (spawnPointIndex == -1)
        {
            spawnPointIndex = Random.Range(0, spawnPoints.Count);
        }
        
        SpawnWithSpeed(0, spawnPointIndex, speed);
    }
    
    // Spawn debris with specific speed
    public void SpawnDebrisWithSpeed(int spawnPointIndex, float speed)
    {
        if (spawnPointIndex == -1)
        {
            spawnPointIndex = Random.Range(0, spawnPoints.Count);
        }
        
        SpawnWithSpeed(1, spawnPointIndex, speed);
    }
}
