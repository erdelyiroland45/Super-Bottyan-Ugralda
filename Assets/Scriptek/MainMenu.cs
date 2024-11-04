using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  // Include this namespace for SceneManager

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // Use SceneManager to load the scene instead of Application.LoadLevel
        SceneManager.LoadScene("M_Epulet");  // Make sure "SampleScene" matches your scene's name
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");  // Log message when quitting
        Application.Quit();   // Quit the application
    }
}
