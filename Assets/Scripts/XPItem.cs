using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPItem : MonoBehaviour {
    public int xpAmount = 10; // Default amount of XP this item gives
    private float despawnTime = 30f; // Time in seconds before the item disappears

    private void Start() {
        // Automatically destroy the XP item after `despawnTime` seconds
        Destroy(gameObject, despawnTime);
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