using UnityEngine;
using System.Collections.Generic;

public class TimelineSpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    public List<Transform> spawnPoints = new List<Transform>();
    
    [Header("Prefabs")]
    public GameObject snowPrefab;
    public GameObject debrisPrefab;
    public List<GameObject> additionalPrefabs = new List<GameObject>();
    
    // Methods that will be called directly from Timeline signals
    
    // Spawn snow at a specific spawn point
    public void SpawnSnow(int spawnPointIndex)
    {
        if (ValidateSpawnPoint(spawnPointIndex) && snowPrefab != null)
        {
            Instantiate(snowPrefab, spawnPoints[spawnPointIndex].position, Quaternion.identity);
        }
    }
    
    // Spawn debris at a specific spawn point
    public void SpawnDebris(int spawnPointIndex)
    {
        if (ValidateSpawnPoint(spawnPointIndex) && debrisPrefab != null)
        {
            Instantiate(debrisPrefab, spawnPoints[spawnPointIndex].position, Quaternion.identity);
        }
    }
    
    // Spawn a specific prefab from the additional prefabs list
    public void SpawnPrefab(int prefabIndex, int spawnPointIndex)
    {
        if (ValidateSpawnPoint(spawnPointIndex) && 
            prefabIndex >= 0 && prefabIndex < additionalPrefabs.Count)
        {
            Instantiate(additionalPrefabs[prefabIndex], spawnPoints[spawnPointIndex].position, Quaternion.identity);
        }
    }
    
    // Spawn snow at a random spawn point
    public void SpawnSnowRandom()
    {
        if (spawnPoints.Count > 0 && snowPrefab != null)
        {
            int randomIndex = Random.Range(0, spawnPoints.Count);
            Instantiate(snowPrefab, spawnPoints[randomIndex].position, Quaternion.identity);
        }
    }
    
    // Spawn debris at a random spawn point
    public void SpawnDebrisRandom()
    {
        if (spawnPoints.Count > 0 && debrisPrefab != null)
        {
            int randomIndex = Random.Range(0, spawnPoints.Count);
            Instantiate(debrisPrefab, spawnPoints[randomIndex].position, Quaternion.identity);
        }
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
    
    // Called when the GameObject is first created
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
}
