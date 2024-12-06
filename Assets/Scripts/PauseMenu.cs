using System.Collections;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [Header("Canvas References")]
    public GameObject mainCanvas;      // Reference to the Main Game Canvas
    public GameObject helpMenuCanvas;  // Reference to the Help Menu Canvas
    public GameObject pauseMenuPanel;  // Reference to the Pause Menu Panel (Child of Main Canvas)

    private bool isPaused = false;

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0; // Pause the game
            mainCanvas.SetActive(true); // Ensure the main canvas is active
            pauseMenuPanel.SetActive(true); // Show pause menu
        }
        else
        {
            Time.timeScale = 1; // Resume the game
            pauseMenuPanel.SetActive(false); // Hide pause menu
            helpMenuCanvas.SetActive(false); // Ensure help menu is off
            mainCanvas.SetActive(true); // Keep main canvas active
        }
    }

    public void OpenHelpMenu()
    {
        if (mainCanvas != null && helpMenuCanvas != null)
        {
            pauseMenuPanel.SetActive(false); // Deactivate Pause Menu
            mainCanvas.SetActive(false); // Deactivate Main Canvas
            helpMenuCanvas.SetActive(true); // Activate Help Menu
        }
    }

    public void CloseHelpMenu()
    {
        if (mainCanvas != null && helpMenuCanvas != null)
        {
            helpMenuCanvas.SetActive(false); // Deactivate Help Menu
            mainCanvas.SetActive(true); // Reactivate Main Canvas
            pauseMenuPanel.SetActive(true); // Reactivate Pause Menu
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in editor
        #endif
    }

    public void ResumeGame()
    {
        // Ensure we toggle the pause state and properly reset canvases
        Time.timeScale = 1; // Resume game time
        FindObjectOfType<PlayerController>().TogglePause();
        pauseMenuPanel.SetActive(false); // Hide Main Canvas
    }
}