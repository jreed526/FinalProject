using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {
    [Header("General")]
    private CharacterController controller;
    [SerializeField] private float speed = 5f;
    [SerializeField] private Transform cam;
    [SerializeField] private GameObject projectilePrefab;  // Reference to projectile prefab
    [SerializeField] private Transform projectileSpawnPoint;  // Spawn point of the projectile
    [SerializeField] private float rotationSpeed = 700f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float fireRate = 1f;  // Time in seconds between shots

    private Vector3 velocity;

    private void Awake() {
        controller = GetComponent<CharacterController>();
    }

    private void Start() {
        // Start the auto-firing coroutine
        StartCoroutine(AutoFire());
    }

    void Update() {
        // Get input for movement
        float x = Input.GetAxis("Horizontal"); 
        float z = Input.GetAxis("Vertical");   

        // Get camera-relative forward direction, ignoring the y-axis
        Vector3 forward = cam.forward;
        forward.y = 0f;
        forward.Normalize();

        // Get camera-relative right direction, ignoring the y-axis
        Vector3 right = cam.right;
        right.y = 0f;
        right.Normalize();

        // Combine the input with the camera directions
        Vector3 move = forward * z + right * x;

        // Move the character controller
        controller.Move(move * speed * Time.deltaTime);

        // Rotate the player in the direction of movement
        if (move.magnitude > 0) {
            Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Apply gravity (if necessary for specific behaviors)
        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private IEnumerator AutoFire() {
        while (true) {
            ShootProjectile();
            yield return new WaitForSeconds(fireRate);  // Waits for 1 second (or specified fireRate) before the next shot
        }
    }

    private void ShootProjectile() {
        // Instantiate a new projectile at the spawn point and facing direction
        Instantiate(projectilePrefab, projectileSpawnPoint.position, transform.rotation);
    }
}

