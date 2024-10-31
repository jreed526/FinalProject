using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour {
    public float speed = 2f;
    private Transform hero;   // Reference to the hero's Transform

    void Start() {
        // Dynamically find the hero in the scene
        GameObject heroObject = GameObject.FindGameObjectWithTag("Hero");
        if (heroObject != null) {
            hero = heroObject.transform;
        } else {
            Debug.LogError("Hero object with tag 'Hero' not found in the scene!");
        }
    }

    void Update() {
        if (hero != null) {
            // Move directly towards the hero's current position
            transform.position = Vector3.MoveTowards(transform.position, hero.position, speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Hero")) {
            Destroy(gameObject); // Destroy the enemy
            // Additional effects for when the enemy touches the hero can go here
        } else if (other.CompareTag("Projectile")) {
            Destroy(gameObject); // Destroy the enemy
            Destroy(other.gameObject); // Destroy the projectile
            // Additional effects for when the enemy is hit by a projectile
        }
    }
}

