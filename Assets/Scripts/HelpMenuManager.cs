using UnityEngine;

public class HelpMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainHelpMenu; // Main menu with "How to Play" and "Items/Skills" buttons
    public GameObject howToPlayPanel; // Panel for "How to Play" information
    public GameObject itemsSkillsPanel; // Panel for "Items/Skills" information

    [Header("Other Canvases")]
    public GameObject mainMenuCanvas; // Reference to the Start Screen canvas

    void Start()
    {
        // Ensure only the main menu is active by default
        ShowMainHelpMenu();
    }

    // Show the main help menu
    public void ShowMainHelpMenu()
    {
        mainHelpMenu.SetActive(true);
        howToPlayPanel.SetActive(false);
        itemsSkillsPanel.SetActive(false);
    }

    // Show the "How to Play" panel
    public void ShowHowToPlayPanel()
    {
        mainHelpMenu.SetActive(false);
        howToPlayPanel.SetActive(true);
    }

    // Show the "Items/Skills" panel
    public void ShowItemsSkillsPanel()
    {
        mainHelpMenu.SetActive(false);
        itemsSkillsPanel.SetActive(true);
    }

    // Close the help menu (go back to the previous screen)
    public void CloseHelpMenu()
    {
        gameObject.SetActive(false);
    }

    // Return to the main menu canvas
    public void ReturnToMainMenu()
    {
        // Deactivate the Help Menu
        gameObject.SetActive(false);

        // Activate the Start Screen
        if (mainMenuCanvas != null)
        {
            mainHelpMenu.SetActive(false);
            mainMenuCanvas.SetActive(true);
        }
    }
}
