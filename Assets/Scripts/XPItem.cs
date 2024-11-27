using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPItem : MonoBehaviour {
    public int xpAmount = 10; // Default amount of XP this item gives
    private float despawnTime = 30f; // Time in seconds before the item disappears
    private Transform heroTransform; // Reference to the hero's transform
    public float attractionRadius = 5f; // Radius within which the XP item is attracted to the hero
    public float attractionSpeed = 3f; // Speed at which the XP item moves towards the hero

    private void Start() {
        // Automatically destroy the XP item after `despawnTime` seconds
        Destroy(gameObject, despawnTime);

        // Find the hero in the scene
        GameObject hero = GameObject.FindGameObjectWithTag("Hero");
        if (hero != null) {
            heroTransform = hero.transform;
        } else {
            Debug.LogError("Hero not found in the scene!");
        }
    }

    private void Update() {
        if (heroTransform == null) return;

        // Calculate the distance between the XP item and the hero
        float distanceToHero = Vector3.Distance(transform.position, heroTransform.position);

        // If within the attraction radius, move towards the hero
        if (distanceToHero <= attractionRadius) {
            Vector3 direction = (heroTransform.position - transform.position).normalized;
            transform.position += direction * attractionSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other) {
        // Check if the object that collided with this XP item has the "Hero" tag
        if (other.CompareTag("Hero")) {
            // Try to get the PlayerController component from the hero to add XP
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null) {
                playerController.AddXP(xpAmount); // Add XP to the player's total
            }

            Destroy(gameObject); // Destroy the XP item after it's collected
        }
    }

    // New method to set the XP amount
    public void SetXPAmount(int amount) {
        xpAmount = amount;
    }
}
