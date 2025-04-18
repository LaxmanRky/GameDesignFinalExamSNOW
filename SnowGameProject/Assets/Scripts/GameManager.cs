using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    // Static instance for singleton pattern
    public static GameManager Instance { get; private set; }
    
    [Header("Player")]
    public GameObject astronautPrefab;
    public Transform spawnPoint;
    
    [Header("Score System")]
    public TMP_Text currentScoreText;
    public TMP_Text highScoreText;
    private int currentScore = 0;
    private int highScore = 0;
    
    [Header("Health System")]
    public Slider healthSlider;
    public TMP_Text healthText;
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public float debrisDamage = 10f; // Damage taken when hit by debris
    
    [Header("Water Level System")]
    public Slider waterLevelSlider;
    public TMP_Text waterLevelText;
    public float maxWaterLevel = 100f;
    public float currentWaterLevel = 0f;
    public float snowWaterIncrease = 20f; // Water gained when collecting snow
    
    [Header("Game State")]
    public GameObject gameOverPanel;
    public GameObject levelCompletePanel;
    
    private bool isGameActive = false;
    private float scoreTimer = 0f;
    
    void Awake()
    {
        Debug.Log("GameManager Awake called");
        
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager set as DontDestroyOnLoad singleton");
        }
        else if (Instance != this)
        {
            Debug.Log("Destroying duplicate GameManager");
            Destroy(gameObject);
            return;
        }
        
        // Load high score from PlayerPrefs
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        
        // Initialize game state
        isGameActive = true;
        currentHealth = maxHealth;
        currentWaterLevel = 0f;
        
        Debug.Log($"GameManager initialized with health: {currentHealth}, water: {currentWaterLevel}");
    }
    
    void Start()
    {
        // Initialize health and water level UI
        UpdateHealthUI();
        UpdateWaterLevelUI();
        
        // Hide game state panels
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        
        // Start the game
        StartGame();
    }
    
    void Update()
    {
        if (isGameActive)
        {
            // Increase score over time
            scoreTimer += Time.deltaTime;
            if (scoreTimer >= 1f)
            {
                scoreTimer = 0f;
                currentScore++;
                UpdateScoreText();
                
                // Check if we have a new high score
                if (currentScore > highScore)
                {
                    highScore = currentScore;
                    PlayerPrefs.SetInt("HighScore", highScore);
                    UpdateHighScoreText();
                }
            }
        }
    }
    
    void StartGame()
    {
        isGameActive = true;
        currentScore = 0;
        currentHealth = maxHealth;
        currentWaterLevel = 0f;
        
        UpdateScoreText();
        UpdateHealthUI();
        UpdateWaterLevelUI();
        
        // Hide game state panels
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        
        // Spawn the astronaut if it doesn't exist
        if (spawnPoint != null && astronautPrefab != null && GameObject.FindGameObjectWithTag("Player") == null)
        {
            Instantiate(astronautPrefab, spawnPoint.position, Quaternion.identity);
        }
    }
    
    // Method to add points to the score
    public void AddPoints(int points = 1)
    {
        currentScore += points;
        UpdateScoreText();
        
        // Check if we have a new high score
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            UpdateHighScoreText();
        }
    }
    
    void UpdateScoreText()
    {
        if (currentScoreText != null)
        {
            currentScoreText.text = "Score: " + currentScore;
        }
    }
    
    void UpdateHighScoreText()
    {
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + highScore;
        }
    }
    
    // Health methods
    public void TakeDamage(float damage)
    {
        Debug.Log($"TakeDamage called with damage: {damage}, current health: {currentHealth}");
        
        // Always allow damage even if game is not active
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth); // Prevent negative health
        UpdateHealthUI();
        
        Debug.Log($"Health after damage: {currentHealth}");
        
        // Check for game over
        if (currentHealth <= 0)
        {
            GameOver();
        }
    }
    
    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
        }
        
        if (healthText != null)
        {
            healthText.text = "Health: " + Mathf.Round(currentHealth) + "%";
        }
    }
    
    // Water level methods
    public void AddWater(float amount)
    {
        Debug.Log($"AddWater called with amount: {amount}, current water: {currentWaterLevel}");
        
        // Always allow water collection even if game is not active
        currentWaterLevel += amount;
        currentWaterLevel = Mathf.Min(maxWaterLevel, currentWaterLevel); // Cap at max water level
        UpdateWaterLevelUI();
        
        Debug.Log($"Water after collection: {currentWaterLevel}");
        
        // Check for level complete
        if (currentWaterLevel >= maxWaterLevel && isGameActive)
        {
            LevelComplete();
        }
    }
    
    void UpdateWaterLevelUI()
    {
        if (waterLevelSlider != null)
        {
            waterLevelSlider.value = currentWaterLevel / maxWaterLevel;
        }
        
        if (waterLevelText != null)
        {
            waterLevelText.text = "Water: " + Mathf.Round(currentWaterLevel) + "%";
        }
    }
    
    public void GameOver()
    {
        isGameActive = false;
        
        // Pause the game
        Time.timeScale = 0f;
        
        // Show game over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        Debug.Log("Game Over!");
    }
    
    public void LevelComplete()
    {
        isGameActive = false;
        
        // Pause the game
        Time.timeScale = 0f;
        
        // Show level complete panel
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }
        
        Debug.Log("Level Complete!");
    }
    
    // Public method to restart the game
    public void RestartGame()
    {
        // Unpause the game
        Time.timeScale = 1f;
        
        // Hide game over and level complete panels
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        
        // Reset score, health and water level
        currentScore = 0;
        currentHealth = maxHealth;
        currentWaterLevel = 0f;
        scoreTimer = 0f;
        isGameActive = true;
        
        // Update UI
        UpdateScoreText();
        UpdateHealthUI();
        UpdateWaterLevelUI();
        
        // Destroy all falling objects
        DestroyAllFallingObjects();
        
        // Reset astronaut position
        ResetAstronaut();
        
        // Find and play the timeline
        PlayableDirector[] directors = FindObjectsByType<PlayableDirector>(FindObjectsSortMode.None);
        foreach (PlayableDirector director in directors)
        {
            // Stop first to reset the timeline to the beginning
            director.Stop();
            // Then play to restart it
            director.Play();
            Debug.Log($"Restarted timeline: {director.gameObject.name}");
        }
        
        // Find any TimelineSignalReceiver and tell it to start auto spawning
        TimelineSignalReceiver[] receivers = FindObjectsByType<TimelineSignalReceiver>(FindObjectsSortMode.None);
        foreach (TimelineSignalReceiver receiver in receivers)
        {
            receiver.StartAutoSpawning();
            Debug.Log($"Started auto spawning on: {receiver.gameObject.name}");
        }
        
        Debug.Log("Game Restarted - All values reset to initial state");
    }
    
    private void DestroyAllFallingObjects()
    {
        // Find and destroy all falling objects regardless of tag
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int destroyedCount = 0;
        
        foreach (GameObject obj in allObjects)
        {
            // Check if it's a falling object by looking for relevant components or names
            if (obj.GetComponent<SimpleFallingObject>() != null || 
                obj.GetComponent<FallingObjectController>() != null ||
                obj.GetComponent<SnowController>() != null ||
                obj.GetComponent<DebrisController>() != null ||
                obj.name.ToLower().Contains("snow") ||
                obj.name.ToLower().Contains("debris") ||
                obj.name.ToLower().Contains("asteroid"))
            {
                Destroy(obj);
                destroyedCount++;
            }
        }
        
        Debug.Log($"Destroyed {destroyedCount} falling objects");
    }
    
    private void ResetAstronaut()
    {
        GameObject astronaut = GameObject.FindGameObjectWithTag("Player");
        if (astronaut != null)
        {
            // Reset astronaut position if spawn point is defined
            if (spawnPoint != null)
            {
                astronaut.transform.position = spawnPoint.position;
                Debug.Log("Reset astronaut position to spawn point");
            }
            
            // Enable astronaut controller
            AstronautController controller = astronaut.GetComponent<AstronautController>();
            if (controller != null)
            {
                controller.enabled = true;
                
                // Reset any state in the controller
                controller.SendMessage("ResetState", null, SendMessageOptions.DontRequireReceiver);
            }
        }
        else if (spawnPoint != null && astronautPrefab != null)
        {
            // Spawn a new astronaut if none exists
            astronaut = Instantiate(astronautPrefab, spawnPoint.position, Quaternion.identity);
            Debug.Log("Created new astronaut at spawn point");
        }
    }
    
    private void ResetSpawners()
    {
        // Find and reset any spawners
        var spawners = FindObjectsOfType<FallingObjectSpawnPoint>();
        foreach (var spawner in spawners)
        {
            spawner.SendMessage("ResetSpawner", null, SendMessageOptions.DontRequireReceiver);
        }
        
        // Find and reset any timeline managers
        var timelineManagers = FindObjectsOfType<TimelineManager>();
        
        if (timelineManagers.Length > 0)
        {
            foreach (var manager in timelineManagers)
            {
                // Directly call the ResetTimeline method
                manager.ResetTimeline();
            }
            Debug.Log($"Reset {spawners.Length} spawners and {timelineManagers.Length} timeline managers");
        }
        else
        {
            Debug.LogWarning("No TimelineManager found! Creating one to ensure objects spawn...");
            
            // Create a new TimelineManager if none exists
            GameObject timelineObj = new GameObject("TimelineManager");
            TimelineManager newManager = timelineObj.AddComponent<TimelineManager>();
            
            // Try to find spawn points
            Transform spawnPointsParent = GameObject.Find("SpawnPoints")?.transform;
            if (spawnPointsParent != null)
            {
                foreach (Transform child in spawnPointsParent)
                {
                    newManager.spawnPoints.Add(child);
                }
                Debug.Log($"Found {newManager.spawnPoints.Count} spawn points for new TimelineManager");
            }
            else
            {
                // Create a default spawn point if none exist
                GameObject spawnPointsObj = new GameObject("SpawnPoints");
                GameObject defaultSpawn = new GameObject("DefaultSpawnPoint");
                defaultSpawn.transform.parent = spawnPointsObj.transform;
                defaultSpawn.transform.position = new Vector3(0, 10, 0); // High above the scene
                newManager.spawnPoints.Add(defaultSpawn.transform);
                Debug.Log("Created default spawn point for new TimelineManager");
            }
            
            // Find prefabs to spawn
            newManager.snowPrefabs = FindSnowPrefabs();
            newManager.debrisPrefabs = FindDebrisPrefabs();
            
            // Start spawning
            newManager.StartSpawning();
            Debug.Log("Created and started new TimelineManager");
        }
    }
    
    private List<GameObject> FindSnowPrefabs()
    {
        List<GameObject> prefabs = new List<GameObject>();
        
        // Try to find existing snow prefabs in the scene
        TimelineManager existingManager = FindObjectOfType<TimelineManager>();
        if (existingManager != null && existingManager.snowPrefabs.Count > 0)
        {
            // Copy prefabs from existing manager
            prefabs.AddRange(existingManager.snowPrefabs);
            Debug.Log($"Found {prefabs.Count} snow prefabs from existing TimelineManager");
            return prefabs;
        }
        
        // Try to find prefabs by name in the scene
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.ToLower().Contains("snow") || obj.name.ToLower().Contains("flake"))
            {
                // Check if this is a prefab instance (has "(Clone)" in name)
                if (obj.name.Contains("(Clone)"))
                {
                    // Get the prefab from the instance
                    string prefabName = obj.name.Replace("(Clone)", "").Trim();
                    GameObject prefab = Resources.Load<GameObject>(prefabName);
                    if (prefab != null && !prefabs.Contains(prefab))
                    {
                        prefabs.Add(prefab);
                        Debug.Log($"Found snow prefab from instance: {prefabName}");
                    }
                    else
                    {
                        // If we can't find the prefab, just use the instance itself
                        prefabs.Add(obj);
                        Debug.Log($"Using snow instance as prefab: {obj.name}");
                    }
                }
            }
        }
        
        // Try to find snow prefabs in the Resources folder
        GameObject[] resourcePrefabs = Resources.LoadAll<GameObject>("Prefabs/Snow");
        if (resourcePrefabs.Length > 0)
        {
            foreach (GameObject prefab in resourcePrefabs)
            {
                if (!prefabs.Contains(prefab))
                {
                    prefabs.Add(prefab);
                }
            }
            Debug.Log($"Found {resourcePrefabs.Length} snow prefabs in Resources folder");
        }
        
        // If no prefabs found, create a basic one
        if (prefabs.Count == 0)
        {
            // Look for any snowflake objects in the scene to copy
            GameObject snowflakeTemplate = GameObject.Find("snowflake_0");
            if (snowflakeTemplate != null)
            {
                prefabs.Add(snowflakeTemplate);
                Debug.Log("Found snowflake_0 in scene to use as prefab");
            }
            else
            {
                // Create a basic prefab as last resort
                GameObject basicSnow = new GameObject("BasicSnow");
                basicSnow.AddComponent<SpriteRenderer>();
                basicSnow.AddComponent<SnowController>();
                basicSnow.tag = "Snow";
                
                // Add a collider
                BoxCollider2D collider = basicSnow.AddComponent<BoxCollider2D>();
                collider.isTrigger = true;
                
                // Save the prefab
                prefabs.Add(basicSnow);
                Debug.Log("Created basic snow prefab as last resort");
            }
        }
        
        return prefabs;
    }
    
    private List<GameObject> FindDebrisPrefabs()
    {
        List<GameObject> prefabs = new List<GameObject>();
        
        // Try to find existing debris prefabs in the scene
        TimelineManager existingManager = FindObjectOfType<TimelineManager>();
        if (existingManager != null && existingManager.debrisPrefabs.Count > 0)
        {
            // Copy prefabs from existing manager
            prefabs.AddRange(existingManager.debrisPrefabs);
            Debug.Log($"Found {prefabs.Count} debris prefabs from existing TimelineManager");
            return prefabs;
        }
        
        // Try to find prefabs by name in the scene
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.ToLower().Contains("debris") || obj.name.ToLower().Contains("aster"))
            {
                // Check if this is a prefab instance (has "(Clone)" in name)
                if (obj.name.Contains("(Clone)"))
                {
                    // Get the prefab from the instance
                    string prefabName = obj.name.Replace("(Clone)", "").Trim();
                    GameObject prefab = Resources.Load<GameObject>(prefabName);
                    if (prefab != null && !prefabs.Contains(prefab))
                    {
                        prefabs.Add(prefab);
                        Debug.Log($"Found debris prefab from instance: {prefabName}");
                    }
                    else
                    {
                        // If we can't find the prefab, just use the instance itself
                        prefabs.Add(obj);
                        Debug.Log($"Using debris instance as prefab: {obj.name}");
                    }
                }
            }
        }
        
        // Try to find debris prefabs in the Resources folder
        GameObject[] resourcePrefabs = Resources.LoadAll<GameObject>("Prefabs/Debris");
        if (resourcePrefabs.Length > 0)
        {
            foreach (GameObject prefab in resourcePrefabs)
            {
                if (!prefabs.Contains(prefab))
                {
                    prefabs.Add(prefab);
                }
            }
            Debug.Log($"Found {resourcePrefabs.Length} debris prefabs in Resources folder");
        }
        
        // If no prefabs found, create a basic one
        if (prefabs.Count == 0)
        {
            // Look for any asteroid objects in the scene to copy
            GameObject asteroidTemplate = GameObject.Find("AsterSmall1");
            if (asteroidTemplate != null)
            {
                prefabs.Add(asteroidTemplate);
                Debug.Log("Found AsterSmall1 in scene to use as prefab");
            }
            else
            {
                // Create a basic prefab as last resort
                GameObject basicDebris = new GameObject("BasicDebris");
                basicDebris.AddComponent<SpriteRenderer>();
                basicDebris.AddComponent<DebrisController>();
                basicDebris.tag = "Debris";
                
                // Add a collider
                BoxCollider2D collider = basicDebris.AddComponent<BoxCollider2D>();
                collider.isTrigger = true;
                
                // Save the prefab
                prefabs.Add(basicDebris);
                Debug.Log("Created basic debris prefab as last resort");
            }
        }
        
        return prefabs;
    }
    
    // Method to return to the main menu
    public void ReturnToMainMenu()
    {
        // Unpause the game before switching scenes
        Time.timeScale = 1f;
        
        // Load the main menu scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
    
    // Method to reset the game state without restarting the current scene
    public void ResetGameState()
    {
        Debug.Log("ResetGameState called - resetting all game values");
        
        // Reset game state values
        isGameActive = true;
        currentScore = 0;
        currentHealth = maxHealth;
        currentWaterLevel = 0f;
        scoreTimer = 0f;
        
        // Update UI if available
        if (healthSlider != null) UpdateHealthUI();
        if (waterLevelSlider != null) UpdateWaterLevelUI();
        if (currentScoreText != null) UpdateScoreText();
        if (highScoreText != null) UpdateHighScoreText();
        
        // Hide panels if available
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        
        // Make sure time scale is normal
        Time.timeScale = 1f;
        
        Debug.Log($"Game state reset: Health={currentHealth}, Water={currentWaterLevel}, Score={currentScore}");
    }
}
