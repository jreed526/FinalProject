using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy4 : EnemyBehavior
{
    [Header("Enemy4 Specific Settings")]
    public GameObject projectilePrefab; // Reference to the projectile prefab
    public float projectileSpeed = 5f;
    public float fireRate = 3f; // Time between each shot (very slow)
    public float auraRadius = 2f; // Radius for aura attack
    private float damageTimer = 0f;

    private float fireCooldown = 0f; // Cooldown timer for projectile attack

    protected override void Start()
    {
        base.Start();
        health = 10; // Set specific health for Enemy4
        speed = 1.5f; // Slow movement speed
        shouldDieOnHeroContact = false; // Should not die on hero contact
    }

        protected override void Update()
    {
        base.Update();

        // Always face the hero
        FaceHero();

        // Handle projectile attack with a cooldown
        fireCooldown += Time.deltaTime;
        if (fireCooldown >= fireRate)
        {
            FireProjectiles();
            fireCooldown = 0f;
        }
    }

    // Method to rotate the enemy to face the hero
    private void FaceHero()
    {
        if (hero != null)
        {
            Vector3 directionToHero = hero.position - transform.position;
            directionToHero.y = 0; // Keep the rotation on the horizontal plane (ignore vertical axis)
            if (directionToHero != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(directionToHero);
            }
        }
    }

    // Override the aura attack (similar to Boss1)
    protected override void OnTriggerStay(Collider other)
    {
        base.OnTriggerStay(other);
        if (other.CompareTag("Hero"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                damageTimer += Time.deltaTime;
                if (damageTimer >= 1f)
                {
                    playerController.TakeDamage(1); // Inflict 1 damage every second
                    damageTimer = 0f;
                }
            }
        }
    }

    // Override OnTriggerEnter to prevent dying on contact with the hero
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            Destroy(other.gameObject); // Destroy the projectile
            TakeDamage(1); // Apply damage to the enemy
        }
        else if (other.CompareTag("Hero"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(1); // Hero takes damage on contact
            }
        }
    }

    // Method to shoot 3 projectiles in a V shape
    private void FireProjectiles()
    {
        if (projectilePrefab == null) return;

        // Create 3 projectiles at slightly different angles
        float[] angles = { -15f, 0f, 15f }; // Angles for V shape

        foreach (float angle in angles)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            
            if (rb != null)
            {
                // Apply a forward force with a slight angle
                Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
                rb.velocity = direction * projectileSpeed;
            }
        }
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (health <= 0)
        {
            Die(); // Die only when health reaches 0
        }
    }
}