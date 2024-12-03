using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : EnemyBehavior {
    [Header("Dog Knight Specific Settings")]
    public float auraRadius = 2f; // Aura radius for the Dog Knight's attack effect
    private float damageTimer = 0f;

    [Header("XP Drop Settings")]
    public GameObject xpItem5Prefab;  // Reference to the new XPItem5 prefab for 5 XP points
    public float xpDropChance = 1.0f; // 100% drop chance for this item

    protected override void Start() {
        base.Start();
        // Set Dog Knight (Boss1) to NOT die when touching the hero
        shouldDieOnHeroContact = false; // Prevent Boss1 from dying on hero contact
    }

    protected override void Update() {
        base.Update();
    }

    protected override void OnTriggerEnter(Collider other) {
        base.OnTriggerEnter(other);
    }

    protected override void OnTriggerStay(Collider other) {
        base.OnTriggerStay(other);
        if (other.CompareTag("Hero")) {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null) {
                damageTimer += Time.deltaTime;
                if (damageTimer >= 1f) {
                    playerController.TakeDamage(1); // Inflict 1 damage every second
                    damageTimer = 0f;
                }
            }
        }
    }

    // Override Die to only drop the XPItem5 and prevent base XP drop
    protected override void Die() {
        if (!hasDied) {
            hasDied = true; // Prevent multiple deaths

            // Do NOT call base.Die(); to avoid dropping the base XP prefab
            // Drop only the custom 5 XP item
            if (xpItem5Prefab != null && Random.value <= xpDropChance) {
                Instantiate(xpItem5Prefab, transform.position, Quaternion.identity);
                Instantiate(xpItem5Prefab, transform.position, Quaternion.identity);
            }

            // Destroy the Boss1 object
            Destroy(gameObject);
        }
    }
}