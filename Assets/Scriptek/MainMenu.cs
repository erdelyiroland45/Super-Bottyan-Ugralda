using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // Use SceneManager to load the scene instead of Application.LoadLevel
        SceneManager.LoadScene("M_Epulet");  // Make sure "SampleScene" matches your scene's name
        Time.timeScale = 1f;

        // Reset the player's position to the last checkpoint
        if (CheckpointManager.Instance != null)
        {
            CheckpointManager.Instance.SetCheckpoint(Vector3.zero, "M_Epulet"); // Reset checkpoint for new game
        }
    }

    public void Restart()
    {
        // Load the last scene
        if (CheckpointManager.Instance != null && !string.IsNullOrEmpty(CheckpointManager.Instance.LastSceneName))
        {
            SceneManager.LoadScene(CheckpointManager.Instance.LastSceneName);
        }
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");  // Log message when quitting
        Application.Quit();   // Quit the application
    }

    public void BackMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}