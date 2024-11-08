using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy3 : EnemyBehavior {
    [Header("Dog Knight Specific Settings")]
    public float auraRadius = 2f;
    private float damageTimer = 0f;

    protected override void Start() {
        base.Start();
        // Set Dog Knight to NOT die when touching the hero
        shouldDieOnHeroContact = false; // Prevent Dog Knight from dying on hero contact
    }

    protected override void Update() {
        base.Update();
        // Additional logic for the aura and other behavior
    }

    protected override void OnTriggerEnter(Collider other) {
        base.OnTriggerEnter(other);
        if (other.CompareTag("Hero")) {
            // Additional logic for touching the hero if needed
        }
    }

    // Override OnTriggerStay to apply damage when inside aura
    protected override void OnTriggerStay(Collider other) {
        base.OnTriggerStay(other);
        if (other.CompareTag("Hero")) {
            // Hero is within the aura, apply damage every 1 second
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null) {
                damageTimer += Time.deltaTime;
                if (damageTimer >= 1f) { // Every 1 second
                    playerController.TakeDamage(1); // Hero takes damage
                    damageTimer = 0f; // Reset timer
                }
            }
        }
    }
}
