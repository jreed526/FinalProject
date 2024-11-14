using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("General")]
    private CharacterController controller;
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private Transform cam;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float rotationSpeed = 700f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private GameObject pauseMenu;

    private float currentSpeed; // Speed affected by perks
    private Vector3 velocity;
    private bool isPaused = false;

    // Hero Traits
    [Header("Hero Traits")]
    [SerializeField] private int attackDamage = 1; // Damage per projectile
    [SerializeField] private int maxHealth = 10;   // Maximum health
    private int currentHealth;
    [SerializeField] private int maxShield = 3;    // Maximum shield
    private int currentShield = 0;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI shieldText; // UI for shield

    // Leveling System
    [Header("Leveling System")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentXP = 0;
    [SerializeField] private int xpToNextLevel = 10;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI xpText;

    // Perk System
    [Header("Perk System")]
    [SerializeField] private GameObject perkCanvas; // Reference to Perk UI

    // Properties to access private fields from other scripts
    public int AttackDamage
    {
        get { return attackDamage; }
        set { attackDamage = Mathf.Clamp(value, 1, 5); }
    }

    public float BaseSpeed
    {
        get { return baseSpeed; }
        set { baseSpeed = value; }
    }

    public float CurrentSpeed
    {
        get { return currentSpeed; }
        set { currentSpeed = Mathf.Clamp(value, baseSpeed, baseSpeed * 1.25f); }
    }

    public int MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }

    public int CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = Mathf.Clamp(value, 0, maxHealth); }
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        currentSpeed = baseSpeed;
        currentHealth = maxHealth;
        UpdateXPDisplay();
        UpdateHealthDisplay();
        UpdateShieldDisplay();

        // Start the auto-firing coroutine
        StartCoroutine(AutoFire());
    }

    private void Update()
    {
        // Check for escape key press to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        // If the game is paused, exit the update method
        if (isPaused) return;

        // Get input for movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 forward = cam.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = cam.right;
        right.y = 0f;
        right.Normalize();

        Vector3 move = forward * z + right * x;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Rotate the player in the direction of movement
        if (move.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Apply gravity
        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }

    private IEnumerator AutoFire()
    {
        while (true)
        {
            if (!isPaused)
                ShootProjectile();
            yield return new WaitForSeconds(fireRate);
        }
    }

    private void ShootProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, transform.rotation);
        // Set damage if projectile has a damage component
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.SetDamage(attackDamage);
        }
    }

    public void AddXP(int amount)
    {
        currentXP += amount;
        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
        UpdateXPDisplay();
    }

    private void LevelUp()
    {
        currentLevel++;
        currentXP -= xpToNextLevel;
        xpToNextLevel += 50; // Scaling XP for next level
        UpdateXPDisplay();

        // Show the Perk Menu
        if (perkCanvas != null)
        {
            perkCanvas.SetActive(true);
            Time.timeScale = 0; // Pause the game when the perk menu is active
        }
    }

    private void UpdateXPDisplay()
    {
        levelText.text = "Level: " + currentLevel;
        xpText.text = "XP: " + currentXP + " / " + xpToNextLevel;
    }

    public void TakeDamage(int damage)
    {
        if (currentShield > 0)
        {
            currentShield--; // Shield absorbs the damage first
        }
        else
        {
            currentHealth -= damage;
        }

        UpdateHealthDisplay();
        UpdateShieldDisplay();

        if (currentHealth <= 0)
        {
            Debug.Log("Hero is dead!");
            // Add game over logic here
        }
    }

    public void AddHealth(int amount)
    {
        CurrentHealth += amount;
        UpdateHealthDisplay();
    }

    public void AddShield(int amount)
    {
        currentShield = Mathf.Min(currentShield + amount, maxShield);
        UpdateShieldDisplay();
    }

    public void UpdateHealthDisplay()
    {
        healthText.text = "Health: " + currentHealth + " / " + maxHealth;
    }

    private void UpdateShieldDisplay()
    {
        shieldText.text = "Shield: " + currentShield;
    }

    public void ClosePerkMenu()
    {
        if (perkCanvas != null)
        {
            perkCanvas.SetActive(false);
            Time.timeScale = 1; // Resume the game
        }
    }
}
