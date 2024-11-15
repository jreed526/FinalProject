using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour {
    [Header("Base Enemy Settings")]
    public float speed = 2f;
    public int health = 1; // Default health for base enemies
    public int xpValue = 1; // XP dropped on death

    protected Transform hero;
    public GameObject xpPrefab;

    // Add a flag to prevent multiple XP drops
    protected bool hasDied = false;

    // Flag to determine if the enemy should die on hero contact
    public bool shouldDieOnHeroContact = true;

    protected virtual void Start() {
        GameObject heroObject = GameObject.FindGameObjectWithTag("Hero");
        if (heroObject != null) {
            hero = heroObject.transform;
        } else {
            Debug.LogError("Hero object with tag 'Hero' not found!");
        }
    }

    protected virtual void Update() {
        if (hero != null) {
            // Move towards the hero
            transform.position = Vector3.MoveTowards(transform.position, hero.position, speed * Time.deltaTime);
        }
    }

    // OnTriggerStay marked virtual to allow overriding
    protected virtual void OnTriggerStay(Collider other) {
        if (other.CompareTag("Hero")) {
            // Default behavior, no action here, can be overridden in derived classes
        }
    }

    protected virtual void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Hero")) {
            // Apply damage to the hero, but don't destroy the enemy unless the flag is true
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null) {
                playerController.TakeDamage(1); // Hero takes damage
            }

            if (shouldDieOnHeroContact) {
                Die(); // Destroy the enemy if it should die on hero contact
            }
        } else if (other.CompareTag("Projectile")) {
            Destroy(other.gameObject); // Destroy the projectile
            TakeDamage(1); // Apply damage to the enemy
        }
    }

    public virtual void TakeDamage(int damage) {
        health -= damage;
        if (health <= 0) {
            Die();
        }
    }

    // Method to handle enemy death and ensure XP drops only once
    public GameObject[] pickupPrefabs; // Assign different pickup prefabs in the Inspector

    protected virtual void Die()
    {
        if (!hasDied)
        {
            hasDied = true;
            // Drop XP
            if (xpPrefab != null && xpValue > 0)
            {
                Instantiate(xpPrefab, transform.position, Quaternion.identity);
            }
        
            // Randomly drop a pickup
            DropPickup();

            Destroy(gameObject);
        }
    }

    protected void DropPickup()
    {
        if (pickupPrefabs != null && pickupPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, pickupPrefabs.Length);
            GameObject pickup = pickupPrefabs[randomIndex];

            // 25% chance to drop a pickup
            if (Random.value < 0.25f)
            {
                Instantiate(pickup, transform.position, Quaternion.identity);
            }
        }
    }
}
