using System.Collections;
using UnityEngine;

public class Enemy2 : EnemyBehavior {
    [Header("Blue Turtle Shell Settings")]
    public GameObject projectilePrefab; // Reference to the projectile prefab
    public float projectileSpeed = 3f; // Speed of the projectile
    public float attackInterval = 2f; // Time between attacks

    private float nextAttackTime;

    protected override void Start() {
        // Call the base Start method to find the hero
        base.Start();
        
        // Set specific attributes for Blue Turtle Shell
        health = 5;
        speed = 1.0f; // Slower movement speed than the base enemy
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
