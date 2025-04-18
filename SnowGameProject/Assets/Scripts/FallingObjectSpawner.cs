using UnityEngine;
using System.Collections.Generic;

public class FallingObjectSpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    public List<Transform> spawnPoints = new List<Transform>();
    
    [Header("Falling Objects")]
    public List<GameObject> fallingObjectPrefabs = new List<GameObject>();
    
    private void Awake()
    {
        // Try to find spawn points if none are assigned
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
    
    // Spawn a specific prefab at a specific spawn point
    // This method can be called directly from Timeline signals
    public void SpawnObject(int prefabIndex, int spawnPointIndex)
    {
        if (!ValidatePrefabIndex(prefabIndex) || !ValidateSpawnPoint(spawnPointIndex))
            return;
            
        Instantiate(fallingObjectPrefabs[prefabIndex], 
                   spawnPoints[spawnPointIndex].position, 
                   Quaternion.identity);
    }
    
    // Spawn a specific prefab at a random spawn point
    public void SpawnObjectRandom(int prefabIndex)
    {
        if (!ValidatePrefabIndex(prefabIndex) || spawnPoints.Count == 0)
            return;
            
        int randomSpawnPoint = Random.Range(0, spawnPoints.Count);
        Instantiate(fallingObjectPrefabs[prefabIndex], 
                   spawnPoints[randomSpawnPoint].position, 
                   Quaternion.identity);
    }
    
    // Spawn a random prefab at a specific spawn point
    public void SpawnRandomObject(int spawnPointIndex)
    {
        if (!ValidateSpawnPoint(spawnPointIndex) || fallingObjectPrefabs.Count == 0)
            return;
            
        int randomPrefab = Random.Range(0, fallingObjectPrefabs.Count);
        Instantiate(fallingObjectPrefabs[randomPrefab], 
                   spawnPoints[spawnPointIndex].position, 
                   Quaternion.identity);
    }
    
    // Spawn a random prefab at a random spawn point
    public void SpawnRandomObjectAtRandomPoint()
    {
        if (fallingObjectPrefabs.Count == 0 || spawnPoints.Count == 0)
            return;
            
        int randomPrefab = Random.Range(0, fallingObjectPrefabs.Count);
        int randomSpawnPoint = Random.Range(0, spawnPoints.Count);
        
        Instantiate(fallingObjectPrefabs[randomPrefab], 
                   spawnPoints[randomSpawnPoint].position, 
                   Quaternion.identity);
    }
    
    // Helper method to validate prefab index
    private bool ValidatePrefabIndex(int index)
    {
        if (fallingObjectPrefabs.Count == 0)
        {
            Debug.LogWarning("No prefabs assigned!");
            return false;
        }
        
        if (index < 0 || index >= fallingObjectPrefabs.Count)
        {
            Debug.LogWarning($"Invalid prefab index: {index}. Valid range is 0-{fallingObjectPrefabs.Count-1}");
            return false;
        }
        
        return true;
    }
    
    // Helper method to validate spawn point index
    private bool ValidateSpawnPoint(int index)
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("No spawn points assigned!");
            return false;
        }
        
        if (index < 0 || index >= spawnPoints.Count)
        {
            Debug.LogWarning($"Invalid spawn point index: {index}. Valid range is 0-{spawnPoints.Count-1}");
            return false;
        }
        
        return true;
    }
}
