using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    [SerializeField] private float speed = 10f;
    [SerializeField] private float maxLifetime = 2f;
    private Rigidbody rb;
    private Camera mainCamera;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }

    private void Start() {
        // Set initial velocity
        rb.velocity = transform.forward * speed;
        
        // Destroy the projectile after a max lifetime if it hasn't hit anything
        Destroy(gameObject, maxLifetime);
    }

    private void Update() {
        // Check if the projectile is out of the camera's view
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
        if (screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Enemy")) {
            Destroy(other.gameObject);  // Destroy enemy on collision
            Destroy(gameObject);  // Destroy projectile on collision
        }
    }
}

