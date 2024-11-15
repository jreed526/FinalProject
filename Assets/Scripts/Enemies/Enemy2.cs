using System.Collections;
using UnityEngine;

public class Enemy2 : EnemyBehavior {
    [Header("Blue Turtle Shell Settings")]
    public GameObject projectilePrefab; // Reference to the projectile prefab
    public float projectileSpeed = 3f; // Speed of the projectile
    public float attackInterval = 2f; // Time between attacks

    public float standardSpeed = 1.0f; // Add this line

    private float nextAttackTime;

    protected override void Start() {
        // Call the base Start method to find the hero
        base.Start();
        
        // Set specific attributes for Blue Turtle Shell
        health = 5;
        speed = standardSpeed; // Use the new standardSpeed variable here
        xpValue = 3; // XP dropped on death
        
        nextAttackTime = Time.time + attackInterval;
    }

    protected override void Update() {
        // Keep the movement behavior from the base class
        base.Update();

        // Handle projectile attacks
        if (Time.time >= nextAttackTime && hero != null) {
            Attack();
            nextAttackTime = Time.time + attackInterval;
        }
    }

    private void Attack() {
        if (projectilePrefab != null) {
            // Instantiate the projectile
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            
            // Calculate direction towards the hero
            Vector3 direction = (hero.position - transform.position).normalized;
            
            // Set the projectile's velocity
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.velocity = direction * projectileSpeed;
            }
        }
    }
}
