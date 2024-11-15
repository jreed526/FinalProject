using System.Collections;
using UnityEngine;

public class Enemy3 : EnemyBehavior {
    [Header("Enemy 3 Settings")]
    public int enemyHealth = 5;
    public float fireRate = 2f; // Time between projectile shots
    public GameObject projectilePrefab; // Assign in the Inspector for the V-shaped attack
    public float projectileSpeed = 5f; // Speed of the projectile
    public float projectileSpreadAngle = 30f; // Angle between the two projectiles

    private float fireCooldown = 0f;

    // Override Start to set Enemy 3-specific values
    protected override void Start() {
        base.Start();
        health = enemyHealth; // Set specific health for Enemy 3
        xpValue = 3; // XP drop value
    }

    protected override void Update() {
        base.Update(); // Handle movement towards the hero

        // Handle projectile attack
        if (fireCooldown <= 0f && hero != null) {
            FireProjectiles();
            fireCooldown = fireRate;
        }

        fireCooldown -= Time.deltaTime;
    }

    // Method to fire two projectiles in a V shape
    private void FireProjectiles() {
        if (projectilePrefab != null) {
            // Calculate direction to the hero
            Vector3 directionToHero = (hero.position - transform.position).normalized;

            // Calculate spread angles
            Vector3 leftSpread = Quaternion.Euler(0, -projectileSpreadAngle, 0) * directionToHero;
            Vector3 rightSpread = Quaternion.Euler(0, projectileSpreadAngle, 0) * directionToHero;

            // Spawn two projectiles with the calculated directions
            SpawnProjectile(leftSpread);
            SpawnProjectile(rightSpread);
        }
    }

    // Helper method to spawn a projectile
    private void SpawnProjectile(Vector3 direction) {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        if (projectileRb != null) {
            projectileRb.velocity = direction * projectileSpeed;
        }
    }

    // Override Die method to customize pickup drop chance
    protected override void Die() {
        if (!hasDied) {
            hasDied = true;
        
            // Drop XP
            if (xpPrefab != null && xpValue > 0) {
                GameObject xpDrop = Instantiate(xpPrefab, transform.position, Quaternion.identity);
            
                // Set the XP amount dynamically
                XPItem xpItemScript = xpDrop.GetComponent<XPItem>();
                if (xpItemScript != null) {
                    xpItemScript.SetXPAmount(xpValue); // Pass the correct xpValue
                }
            }
        
            // Randomly drop a pickup
            DropSpecificPickup();

            Destroy(gameObject);
        }
    }

    // Drop specific pickups with a 0.25% chance for each type
    private void DropSpecificPickup() {
        if (pickupPrefabs != null && pickupPrefabs.Length > 0) {
            foreach (GameObject pickup in pickupPrefabs) {
                if (Random.value < 0.0025f) { // 0.25% chance
                    Instantiate(pickup, transform.position, Quaternion.identity);
                }
            }
        }
    }
}
