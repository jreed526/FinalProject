using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {
    [Header("General")]
    private CharacterController controller;
    [SerializeField] private float speed = 5f;
    [SerializeField] private Transform cam;
    [SerializeField] private GameObject projectilePrefab;  // Reference to projectile prefab
    [SerializeField] private Transform projectileSpawnPoint;  // Spawn point of the projectile
    [SerializeField] private float rotationSpeed = 700f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float fireRate = 1f;  // Time in seconds between shots
    [SerializeField] private GameObject pauseMenu;

    private Vector3 velocity;
    private bool isPaused = false; // Checks if game is paused

    // Leveling System
    [Header("Leveling System")]
    [SerializeField] private int currentLevel = 1;  // Initial player level
    [SerializeField] private int currentXP = 0;     // Current XP
    [SerializeField] private int xpToNextLevel = 10; // XP needed to level up initially
    [SerializeField] private TextMeshProUGUI levelText;  // UI Text to display current level
    [SerializeField] private TextMeshProUGUI xpText;     // UI Text to display XP progress

    // Health System
    [Header("Health System")]
    [SerializeField] private int maxHealth = 10;     // Maximum health
    private int currentHealth;
    [SerializeField] private TextMeshProUGUI healthText; // UI Text to display health

    private void Awake() {
        controller = GetComponent<CharacterController>();
    }

    private void Start() {
        // Start the auto-firing coroutine
        StartCoroutine(AutoFire());

        // Initialize health, XP, and level display
        currentHealth = maxHealth;
        UpdateXPDisplay();
        UpdateHealthDisplay();
    }

    void Update() {
        // Check for escape key press to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape)) {
            TogglePause();
        }

        // If the game is paused, exit the update method
        if (isPaused) return;

        // Get input for movement
        float x = Input.GetAxis("Horizontal"); 
        float z = Input.GetAxis("Vertical");   

        // Get camera-relative forward direction, ignoring the y-axis
        Vector3 forward = cam.forward;
        forward.y = 0f;
        forward.Normalize();

        // Get camera-relative right direction, ignoring the y-axis
        Vector3 right = cam.right;
        right.y = 0f;
        right.Normalize();

        // Combine the input with the camera directions
        Vector3 move = forward * z + right * x;

        // Move the character controller
        controller.Move(move * speed * Time.deltaTime);

        // Rotate the player in the direction of movement
        if (move.magnitude > 0) {
            Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Apply gravity (if necessary for specific behaviors)
        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void TogglePause() {
        isPaused = !isPaused;  // Toggle pause state
        pauseMenu.SetActive(isPaused);  // Show or hide the pause menu
        Time.timeScale = isPaused ? 0 : 1;  // Pause or resume the game time
    }

    private IEnumerator AutoFire() {
        while (true) {
            if (!isPaused)  // Only shoot if the game is not paused
                ShootProjectile();
            yield return new WaitForSeconds(fireRate); 
        }
    }

    private void ShootProjectile() {
        // Instantiate a new projectile at the spawn point and facing direction
        Instantiate(projectilePrefab, projectileSpawnPoint.position, transform.rotation);
    }

    // Method to add XP, which can be called by the XP item on pickup
    public void AddXP(int amount) {
        currentXP += amount;
        Debug.Log("XP added! Current XP: " + currentXP);

        // Check for level up
        if (currentXP >= xpToNextLevel) {
            LevelUp();
        }

        // Update the XP and level display on the UI
        UpdateXPDisplay();
    }

    // Method to handle leveling up
    private void LevelUp() {
        currentLevel++;             // Increase player level
        currentXP -= xpToNextLevel; // Reset current XP for next level
        xpToNextLevel += 50;        // Increase the XP needed for the next level (optional scaling)

        Debug.Log("Leveled up! New Level: " + currentLevel);
        UpdateXPDisplay(); // Update UI to reflect the new level and XP progress
    }

    // Method to update the UI display for level and XP
    private void UpdateXPDisplay() {
        levelText.text = "Level: " + currentLevel;
        xpText.text = "XP: " + currentXP + " / " + xpToNextLevel;
    }

    // Method to take damage when hit by an enemy
    public void TakeDamage(int damage) {
        currentHealth -= damage;
        Debug.Log("Hero took damage! Current Health: " + currentHealth);

        if (currentHealth <= 0) {
            Debug.Log("Hero is dead!");
            // Add game over logic here, like reloading the scene or showing a game over screen
        }

        UpdateHealthDisplay();
    }

    // Method to update the UI display for health
    private void UpdateHealthDisplay() {
        healthText.text = "Health: " + currentHealth;
    }
}

