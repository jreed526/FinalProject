using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {
    public void ResumeGame() {
        FindObjectOfType<PlayerController>().TogglePause();  // Call the toggle method from PlayerController
    }

    public void QuitGame() {
        Application.Quit();  // Quit the game
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;  // Stop playing in editor
        #endif
    }
}

