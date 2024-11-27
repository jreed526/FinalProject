using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))] // Ensures an AudioSource is attached
public class PlayerController : MonoBehaviour
{
    private BlinkColorOnHit blinkColorOnHit;

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

    [Header("Audio")]
    [SerializeField] private AudioClip shootSFX; // Shooting sound effect
    [SerializeField] private AudioClip walkSFX;  // Walking sound effect
    private AudioSource audioSource;            // AudioSource component
    private bool isWalking = false;             // Tracks if the player is walking

    // Hero Traits
    [Header("Hero Traits")]
    [SerializeField] private int attackDamage = 1; // Damage per projectile
    [SerializeField] private int maxHealth = 10;   // Maximum health
    private int currentHealth;
    [SerializeField] private int maxShield = 3;    // Maximum shield
    private int currentShield = 0;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI shieldText; // UI for shield
    private bool isInvincible = false;

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
        audioSource = GetComponent<AudioSource>(); // Initialize AudioSource component
        blinkColorOnHit = GetComponent<BlinkColorOnHit>();
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

        // Check if the player is moving and start/stop walking sound as needed
        if (move.magnitude > 0 && !isWalking) // Start walking sound
        {
            isWalking = true;
            StartCoroutine(PlayWalkingSound());
        }
        else if (move.magnitude == 0 && isWalking) // Stop walking sound
        {
            isWalking = false;
        }

        // Apply gravity
        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Rotate the player to face the mouse
        RotateToMouse();
    }

    private void RotateToMouse()
    {
        // Get the mouse position in screen space
        Vector3 mousePosition = Input.mousePosition;

        // Convert the mouse position to world space
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            Vector3 lookDirection = hit.point - transform.position;
            lookDirection.y = 0; // Ensure the player doesn't tilt up or down
            if (lookDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }

    private bool canShoot = true; // New flag to control shooting

    private IEnumerator AutoFire()
    {
        while (true)
        {
            if (canShoot && !isPaused)
            {
                ShootProjectile();
            }
            yield return new WaitForSeconds(fireRate);
        }
    }

    public void SetShootingEnabled(bool isEnabled)
    {
        canShoot = isEnabled; // Enable or disable shooting
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

        // Play shooting sound effect
        if (shootSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSFX);
        }
    }

    private IEnumerator PlayWalkingSound()
    {
        while (isWalking) // Play walking sound as long as the player is walking
        {
            if (walkSFX != null && audioSource != null)
            {
                audioSource.PlayOneShot(walkSFX);
            }
            else
            {
                Debug.LogWarning("AudioSource or walkSFX is missing!");
            }
            yield return new WaitForSeconds(0.3f); // Interval between footsteps
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

        // Increase the XP needed for the next level by 15 more than the last increase
        xpToNextLevel += (currentLevel * 15) - 5;

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
        if (isInvincible) return;

        // Check if the player has a shield and absorb damage
        if (currentShield > 0)
        {
            currentShield--; // Shield absorbs the damage first
            // Trigger the blue blink when shielded
            if (blinkColorOnHit != null)
            {
                blinkColorOnHit.Blink(true); // Blink blue if shield is present
            }
        }
        else
        {
            currentHealth -= damage;
            // Trigger the red blink when there's no shield
            if (blinkColorOnHit != null)
            {
                blinkColorOnHit.Blink(false); // Blink red if no shield
            }
        }

        UpdateHealthDisplay();
        UpdateShieldDisplay();

        if (currentHealth <= 0)
        {
            Debug.Log("Hero is dead!");
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

    public IEnumerator TempIncreaseAttack(int amount, float duration)
    {
        AttackDamage += amount;
        yield return new WaitForSeconds(duration);
        AttackDamage -= amount; // Revert after duration
    }

    public IEnumerator TempIncreaseSpeed(float speedMultiplier, float duration)
    {
        CurrentSpeed = baseSpeed * (1 + speedMultiplier);
        yield return new WaitForSeconds(duration);
        CurrentSpeed = baseSpeed; // Revert to normal speed
    }

    public IEnumerator ActivateInvincibility(float duration)
    {
        isInvincible = true;
        yield return new WaitForSeconds(duration);
        isInvincible = false;
    }
}
