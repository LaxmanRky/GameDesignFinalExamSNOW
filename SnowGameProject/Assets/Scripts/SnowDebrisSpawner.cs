using UnityEngine;
using System.Collections.Generic;

public class SnowDebrisSpawner : MonoBehaviour
{
    [Header("Falling Objects")]
    public List<GameObject> fallingObjectPrefabs = new List<GameObject>();
    
    // This script is meant to be attached to the TimelineManager
    // It only holds references to the prefabs
    // The actual spawning is handled by SpawnPoint components
    
    private void Awake()
    {
        // Validate prefabs
        if (fallingObjectPrefabs.Count == 0)
        {
            Debug.LogWarning("No prefabs assigned to SnowDebrisSpawner!");
        }
    }
}
