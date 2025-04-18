using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Menu Buttons")]
    public Button startGameButton;
    public Button exitGameButton;
    
    [Header("Scene Settings")]
    public string gameSceneName = "MyGameScene"; // Set this to your game scene name in the Unity Editor
    
    private void Start()
    {
        // Add listeners to buttons
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(StartGame);
        }
        
        if (exitGameButton != null)
        {
            exitGameButton.onClick.AddListener(ExitGame);
        }
    }
    
    // Start the game by loading the game scene
    public void StartGame()
    {
        Debug.Log("Starting game...");
        
        // Reset the GameManager if it exists
        if (GameManager.Instance != null)
        {
            // Reset the GameManager state using the proper method
            GameManager.Instance.ResetGameState();
            Debug.Log("Reset GameManager values before loading game scene");
        }
        
        SceneManager.LoadScene(gameSceneName);
    }
    
    // Exit the game
    public void ExitGame()
    {
        Debug.Log("Exiting game...");
        
        #if UNITY_EDITOR
        // If we are running in the editor
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // If we are running a build
        Application.Quit();
        #endif
    }
}
