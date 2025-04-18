using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimelineManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public List<Transform> spawnPoints = new List<Transform>();
    public List<GameObject> debrisPrefabs = new List<GameObject>();
    public List<GameObject> snowPrefabs = new List<GameObject>();
    
    [Header("Timing Settings")]
    public float minSpawnDelay = 1f;
    public float maxSpawnDelay = 3f;
    public bool autoSpawn = true;
    
    [Header("Spawn Rates")]
    [Range(0, 1)]
    public float snowSpawnChance = 0.7f; // 70% chance to spawn snow, 30% chance to spawn debris
    
    [Header("Debug")]
    public bool showDebugInfo = true;
    
    private bool isSpawning = false;
    
    void Start()
    {
        // Validate spawn points
        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("No spawn points assigned to TimelineManager!");
            
            // Try to find spawn points automatically
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
        
        // Validate prefabs
        if (snowPrefabs.Count == 0 && debrisPrefabs.Count == 0)
        {
            Debug.LogWarning("No prefabs assigned to TimelineManager!");
        }
        
        // Start spawning if auto-spawn is enabled
        if (autoSpawn)
        {
            StartSpawning();
        }
    }
    
    public void StartSpawning()
    {
        if (!isSpawning && (snowPrefabs.Count > 0 || debrisPrefabs.Count > 0) && spawnPoints.Count > 0)
        {
            isSpawning = true;
            StartCoroutine(SpawnRoutine());
        }
    }
    
    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();
    }
    
    private IEnumerator SpawnRoutine()
    {
        while (isSpawning)
        {
            // Wait for a random delay
            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
            
            // Select a random spawn point
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            
            // Determine whether to spawn snow or debris based on probability
            if (Random.value < snowSpawnChance && snowPrefabs.Count > 0)
            {
                // Spawn snow
                GameObject snowPrefab = snowPrefabs[Random.Range(0, snowPrefabs.Count)];
                Instantiate(snowPrefab, spawnPoint.position, Quaternion.identity);
            }
            else if (debrisPrefabs.Count > 0)
            {
                // Spawn debris
                GameObject debrisPrefab = debrisPrefabs[Random.Range(0, debrisPrefabs.Count)];
                Instantiate(debrisPrefab, spawnPoint.position, Quaternion.identity);
            }
        }
    }
    
    // Public method that can be called from Timeline signals or other scripts
    public void SpawnAtPoint(int spawnPointIndex)
    {
        if (spawnPointIndex < 0 || spawnPointIndex >= spawnPoints.Count)
        {
            Debug.LogWarning($"Invalid spawn point index: {spawnPointIndex}");
            return;
        }
        
        Transform spawnPoint = spawnPoints[spawnPointIndex];
        
        // Determine whether to spawn snow or debris based on probability
        if (Random.value < snowSpawnChance && snowPrefabs.Count > 0)
        {
            // Spawn snow
            GameObject snowPrefab = snowPrefabs[Random.Range(0, snowPrefabs.Count)];
            Instantiate(snowPrefab, spawnPoint.position, Quaternion.identity);
        }
        else if (debrisPrefabs.Count > 0)
        {
            // Spawn debris
            GameObject debrisPrefab = debrisPrefabs[Random.Range(0, debrisPrefabs.Count)];
            Instantiate(debrisPrefab, spawnPoint.position, Quaternion.identity);
        }
    }
    
    // Method that can be called from Timeline to spawn specific objects
    public void SpawnSpecificObject(int prefabType, int spawnPointIndex)
    {
        if (spawnPointIndex < 0 || spawnPointIndex >= spawnPoints.Count)
        {
            Debug.LogWarning($"Invalid spawn point index: {spawnPointIndex}");
            return;
        }
        
        Transform spawnPoint = spawnPoints[spawnPointIndex];
        
        // prefabType: 0 = snow, 1 = debris
        if (prefabType == 0 && snowPrefabs.Count > 0)
        {
            // Spawn snow
            GameObject snowPrefab = snowPrefabs[Random.Range(0, snowPrefabs.Count)];
            Instantiate(snowPrefab, spawnPoint.position, Quaternion.identity);
        }
        else if (prefabType == 1 && debrisPrefabs.Count > 0)
        {
            // Spawn debris
            GameObject debrisPrefab = debrisPrefabs[Random.Range(0, debrisPrefabs.Count)];
            Instantiate(debrisPrefab, spawnPoint.position, Quaternion.identity);
        }
    }
    
    // Method to spawn at a completely random spawn point
    public void SpawnAtRandomPoint()
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("No spawn points available for random spawning!");
            return;
        }
        
        // Select a random spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        
        // Determine whether to spawn snow or debris based on probability
        if (Random.value < snowSpawnChance && snowPrefabs.Count > 0)
        {
            // Spawn snow
            GameObject snowPrefab = snowPrefabs[Random.Range(0, snowPrefabs.Count)];
            Instantiate(snowPrefab, spawnPoint.position, Quaternion.identity);
        }
        else if (debrisPrefabs.Count > 0)
        {
            // Spawn debris
            GameObject debrisPrefab = debrisPrefabs[Random.Range(0, debrisPrefabs.Count)];
            Instantiate(debrisPrefab, spawnPoint.position, Quaternion.identity);
        }
    }
    
    // Method to reset the timeline - can be called from GameManager
    public void ResetTimeline()
    {
        // Stop any current spawning
        StopSpawning();
        
        // Clear any existing objects
        GameObject[] snowObjects = GameObject.FindGameObjectsWithTag("Snow");
        foreach (GameObject obj in snowObjects)
        {
            Destroy(obj);
        }
        
        GameObject[] debrisObjects = GameObject.FindGameObjectsWithTag("Debris");
        foreach (GameObject obj in debrisObjects)
        {
            Destroy(obj);
        }
        
        // Restart spawning
        StartSpawning();
        
        Debug.Log("Timeline reset and restarted");
    }
}
