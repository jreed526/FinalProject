using UnityEngine;

public class EnemyProjectile : MonoBehaviour {
    public float damage = 1f;
    public float lifetime = 5f;

    private void Start() {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Hero")) {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null) {
                // Assuming damage is a float, we convert it to int
                playerController.TakeDamage((int)damage); // Explicitly cast float to int
            }
            Destroy(gameObject); // Destroy the projectile on collision
        }
    }
}
