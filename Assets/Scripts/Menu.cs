using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [Header("Canvas References")]
    public GameObject mainMenuCanvas;  // Reference to the Main Menu canvas
    public GameObject helpMenuCanvas; // Reference to the Help Menu canvas
    public GameObject pauseMenuPanel; // Reference to the Pause Menu (child of Main Menu Canvas)
    public GameObject helpMenuMainPanel; // Reference to the Main Panel inside the Help Menu Canvas

    public void OnPlayButton()
    {
        SceneManager.LoadScene("_Scene_0");
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

    public void OnRestartButton()
    {
        SceneManager.LoadScene("_Scene_0");
    }

    public void OnHelpButton()
    {
        if (mainMenuCanvas != null && helpMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(false); // Deactivate Main Menu Canvas
            helpMenuCanvas.SetActive(true); // Activate Help Menu Canvas
            
            // Ensure the Main Panel of the Help Menu is active
            if (helpMenuMainPanel != null)
            {
                helpMenuMainPanel.SetActive(true);
            }
        }
    }

    public void OnBackToPauseMenu()
    {
        if (mainMenuCanvas != null && helpMenuCanvas != null)
        {
            helpMenuCanvas.SetActive(false); // Deactivate Help Menu Canvas
            mainMenuCanvas.SetActive(true); // Reactivate Main Menu Canvas
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(true); // Ensure the pause menu is active
            }
        }
    }
}
