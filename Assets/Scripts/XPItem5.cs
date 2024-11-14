using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPItem5 : MonoBehaviour {
    public int xpAmount = 5;  // Amount of XP this item gives
    private float despawnTime = 30f;  // Time in seconds before the item disappears
    private bool isCollected = false; // Flag to prevent double collection

    private void Start() {
        // Automatically destroy the XP item after `despawnTime` seconds
        Destroy(gameObject, despawnTime);
    }

    private void OnTriggerEnter(Collider other) {
        // Check if the XP item has already been collected
        if (isCollected) return;

        // Check if the object that collided with this XP item has the "Hero" tag
        if (other.CompareTag("Hero")) {
            isCollected = true; // Mark as collected to prevent further triggers

            // Try to get the PlayerController component from the hero to add XP
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null) {
                Debug.Log("Hero collected 5 XP"); // Debug log to check collection
                playerController.AddXP(xpAmount);  // Add XP to the player's total
            }

            Destroy(gameObject);  // Destroy the XP item after it's collected
        }
    }
}

