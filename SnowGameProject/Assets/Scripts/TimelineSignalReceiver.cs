using UnityEngine;
using UnityEngine.Playables;

public class TimelineSignalReceiver : MonoBehaviour
{
    public TimelineManager timelineManager;
    public PlayableDirector playableDirector;
    
    private void Start()
    {
        // Try to find the TimelineManager if not assigned
        if (timelineManager == null)
        {
            timelineManager = FindAnyObjectByType<TimelineManager>();
            if (timelineManager == null)
            {
                Debug.LogError("TimelineSignalReceiver: No TimelineManager found in the scene!");
            }
        }
        
        // Try to find the PlayableDirector if not assigned
        if (playableDirector == null)
        {
            playableDirector = GetComponent<PlayableDirector>();
            if (playableDirector == null)
            {
                Debug.LogError("TimelineSignalReceiver: No PlayableDirector found on this GameObject!");
            }
        }
    }
    
    // Methods that can be called from Timeline signals
    
    public void SpawnSnow(int spawnPointIndex)
    {
        if (timelineManager != null)
        {
            timelineManager.SpawnSpecificObject(0, spawnPointIndex); // 0 = snow
        }
    }
    
    public void SpawnDebris(int spawnPointIndex)
    {
        if (timelineManager != null)
        {
            timelineManager.SpawnSpecificObject(1, spawnPointIndex); // 1 = debris
        }
    }
    
    public void SpawnRandom(int spawnPointIndex = -1)
    {
        if (timelineManager != null)
        {
            // If spawnPointIndex is -1 or invalid, choose a random spawn point
            if (spawnPointIndex == -1)
            {
                timelineManager.SpawnAtRandomPoint();
            }
            else
            {
                timelineManager.SpawnAtPoint(spawnPointIndex);
            }
        }
    }
    
    public void StartAutoSpawning()
    {
        if (timelineManager != null)
        {
            timelineManager.StartSpawning();
        }
    }
    
    public void StopAutoSpawning()
    {
        if (timelineManager != null)
        {
            timelineManager.StopSpawning();
        }
    }
    
    public void PlayTimeline()
    {
        if (playableDirector != null)
        {
            playableDirector.Play();
        }
    }
    
    public void PauseTimeline()
    {
        if (playableDirector != null)
        {
            playableDirector.Pause();
        }
    }
    
    public void StopTimeline()
    {
        if (playableDirector != null)
        {
            playableDirector.Stop();
        }
    }
}
