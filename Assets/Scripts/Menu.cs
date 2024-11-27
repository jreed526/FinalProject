using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [Header("Canvas References")]
    public GameObject mainMenuCanvas;  // Reference to the Main Menu canvas
    public GameObject helpMenuCanvas; // Reference to the Help Menu canvas

    // Called when we click the "Play" button
    public void OnPlayButton()
    {
        SceneManager.LoadScene("_Scene_0");
    }

    // Called when we click the "Quit" button
    public void OnQuitButton()
    {
        Application.Quit();
    }

    // Called when we click the "Restart" button
    public void OnRestartButton()
    {
        SceneManager.LoadScene("_Scene_0");
    }

    // Called when we click the "Help" button
    public void OnHelpButton()
    {
        if (mainMenuCanvas != null && helpMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(false); // Hide the Main Menu
            helpMenuCanvas.SetActive(true); // Show the Help Menu
        }
    }
}
