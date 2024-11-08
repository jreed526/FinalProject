using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour {
    public float speed = 2f;
    private Transform hero;  
    public GameObject xpPrefab;

    void Start() {
        // Find the hero in the scene by tag
        GameObject heroObject = GameObject.FindGameObjectWithTag("Hero");
        if (heroObject != null) {
            hero = heroObject.transform;
        } else {
            Debug.LogError("Hero object with tag 'Hero' not found!");
        }
    }

    void Update() {
        if (hero != null) {
            // Move towards the hero
            transform.position = Vector3.MoveTowards(transform.position, hero.position, speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Hero")) {
            // Damage the hero instead of dropping XP
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null) {
                playerController.TakeDamage(1); // Hero loses 1 life
            }
            Destroy(gameObject); // Destroy the enemy
        } else if (other.CompareTag("Projectile")) {
            // Enemy hit by projectile, drop XP
            Instantiate(xpPrefab, transform.position, Quaternion.identity);
            Destroy(other.gameObject); // Destroy the projectile
            Destroy(gameObject); // Destroy the enemy
        }
    }
}