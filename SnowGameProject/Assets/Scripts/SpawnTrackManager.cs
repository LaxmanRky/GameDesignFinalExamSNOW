using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;

public class SpawnTrackManager : MonoBehaviour
{
    [Header("Spawn Points")]
    public List<Transform> spawnPoints = new List<Transform>();
    
    [Header("Prefabs")]
    public List<GameObject> prefabs = new List<GameObject>();
    
    [Header("Track Configuration")]
    [Tooltip("Each track in the Timeline will use the corresponding spawn point")]
    public bool tracksRepresentSpawnPoints = true;
    
    [Tooltip("Each signal asset will spawn the corresponding prefab")]
    public bool signalsRepresentPrefabs = true;
    
    private PlayableDirector director;
    
    private void Awake()
    {
        director = GetComponent<PlayableDirector>();
        
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
    
    // This will be called by Timeline signals
    public void SpawnPrefab(int prefabIndex)
    {
        // Get the track index from the binding
        int trackIndex = GetTrackIndexFromSignal();
        
        // If tracks represent spawn points, use the track index as the spawn point index
        int spawnPointIndex = tracksRepresentSpawnPoints ? trackIndex : Random.Range(0, spawnPoints.Count);
        
        // Make sure we have valid indices
        if (spawnPointIndex < 0 || spawnPointIndex >= spawnPoints.Count)
        {
            Debug.LogWarning($"Invalid spawn point index: {spawnPointIndex}");
            return;
        }
        
        if (prefabIndex < 0 || prefabIndex >= prefabs.Count)
        {
            Debug.LogWarning($"Invalid prefab index: {prefabIndex}");
            return;
        }
        
        // Spawn the prefab at the spawn point
        Instantiate(prefabs[prefabIndex], spawnPoints[spawnPointIndex].position, Quaternion.identity);
    }
    
    // This will be called by Timeline signals when signalsRepresentPrefabs is true
    public void SpawnFromSignal(string signalName)
    {
        // Get the track index from the binding
        int trackIndex = GetTrackIndexFromSignal();
        
        // If tracks represent spawn points, use the track index as the spawn point index
        int spawnPointIndex = tracksRepresentSpawnPoints ? trackIndex : Random.Range(0, spawnPoints.Count);
        
        // Find the prefab index based on the signal name
        int prefabIndex = -1;
        if (signalName.StartsWith("Spawn"))
        {
            string prefabName = signalName.Substring(5); // Remove "Spawn" prefix
            
            // Try to find a matching prefab by name
            for (int i = 0; i < prefabs.Count; i++)
            {
                if (prefabs[i].name.Contains(prefabName))
                {
                    prefabIndex = i;
                    break;
                }
            }
            
            // If no match found by name, try to parse the index from the signal name
            if (prefabIndex == -1 && int.TryParse(prefabName, out int index))
            {
                prefabIndex = index;
            }
        }
        
        // If we still don't have a valid prefab index, use the first one
        if (prefabIndex < 0 || prefabIndex >= prefabs.Count)
        {
            prefabIndex = 0;
        }
        
        // Make sure we have a valid spawn point index
        if (spawnPointIndex < 0 || spawnPointIndex >= spawnPoints.Count)
        {
            Debug.LogWarning($"Invalid spawn point index: {spawnPointIndex}");
            return;
        }
        
        // Spawn the prefab at the spawn point
        Instantiate(prefabs[prefabIndex], spawnPoints[spawnPointIndex].position, Quaternion.identity);
    }
    
    // Helper method to get the track index from the current signal
    private int GetTrackIndexFromSignal()
    {
        // In a real implementation, you would get this from the Timeline API
        // For now, we'll use a simple approach based on naming conventions
        
        // Get the current signal asset name from the director
        // This is a simplified approach - in a real implementation, you'd use the Timeline API
        string signalName = director.time.ToString();
        
        // Try to extract a track index from the signal name
        int trackIndex = 0;
        
        // For each spawn point, check if its name is in the signal name
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            if (signalName.Contains(spawnPoints[i].name))
            {
                trackIndex = i;
                break;
            }
        }
        
        return trackIndex;
    }
    
    // This can be called directly from a Timeline track
    // The track index will be determined automatically
    public void SpawnRandomPrefab()
    {
        int trackIndex = GetTrackIndexFromSignal();
        int spawnPointIndex = tracksRepresentSpawnPoints ? trackIndex : Random.Range(0, spawnPoints.Count);
        
        if (spawnPointIndex < 0 || spawnPointIndex >= spawnPoints.Count)
        {
            Debug.LogWarning($"Invalid spawn point index: {spawnPointIndex}");
            return;
        }
        
        if (prefabs.Count == 0)
        {
            Debug.LogWarning("No prefabs assigned!");
            return;
        }
        
        int prefabIndex = Random.Range(0, prefabs.Count);
        Instantiate(prefabs[prefabIndex], spawnPoints[spawnPointIndex].position, Quaternion.identity);
    }
}
