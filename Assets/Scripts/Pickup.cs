using System.Collections;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public float attractionRadius = 5f; // Radius within which the pickup is attracted to the hero
    public float attractionSpeed = 3f; // Speed at which the pickup moves towards the hero

    public enum PickupType
    {
        Attack,
        Shield,
        Movement,
        Invincible,
        Health
    }

    [Header("Pickup Settings")]
    public PickupType pickupType;
    public float duration = 10f; // Duration for temporary pickups (e.g., Attack, Movement, Invincible)
    public float despawnTime = 15f; // Time before the pickup despawns
    private Renderer pickupRenderer;
    private Transform heroTransform; // Reference to the hero's transform

    private void Start()
    {
        pickupRenderer = GetComponent<Renderer>();

        // Set color based on pickup type
        switch (pickupType)
        {
            case PickupType.Attack:
                pickupRenderer.material.color = Color.red;
                break;
            case PickupType.Shield:
                pickupRenderer.material.color = Color.blue;
                break;
            case PickupType.Movement:
                pickupRenderer.material.color = new Color(1f, 0.5f, 0f); // Orange
                break;
            case PickupType.Invincible:
                pickupRenderer.material.color = new Color(1f, 0.5f, 1f); // Rainbow-like color
                break;
            case PickupType.Health:
                pickupRenderer.material.color = Color.green;
                break;
        }

        // Find the hero in the scene
        GameObject hero = GameObject.FindGameObjectWithTag("Hero");
        if (hero != null)
        {
            heroTransform = hero.transform;
        }
        else
        {
            Debug.LogError("Hero not found in the scene!");
        }

        // Start the despawn timer
        StartCoroutine(DespawnAfterTime());
    }

    private void Update()
    {
        if (heroTransform == null) return;

        // Calculate the distance between the pickup and the hero
        float distanceToHero = Vector3.Distance(transform.position, heroTransform.position);

        // If within the attraction radius, move towards the hero
        if (distanceToHero <= attractionRadius)
        {
            Vector3 direction = (heroTransform.position - transform.position).normalized;
            transform.position += direction * attractionSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hero"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                ApplyEffect(player);
            }
            Destroy(gameObject); // Destroy pickup after applying effect
        }
    }

    private void ApplyEffect(PlayerController player)
    {
        switch (pickupType)
        {
            case PickupType.Attack:
                player.StartCoroutine(player.TempIncreaseAttack(1, duration));
                break;
            case PickupType.Shield:
                player.AddShield(1);
                break;
            case PickupType.Movement:
                player.StartCoroutine(player.TempIncreaseSpeed(0.15f, duration));
                break;
            case PickupType.Invincible:
                player.StartCoroutine(player.ActivateInvincibility(duration));
                break;
            case PickupType.Health:
                player.AddHealth(2);
                break;
        }
    }

    private IEnumerator DespawnAfterTime()
    {
        yield return new WaitForSeconds(despawnTime);
        Destroy(gameObject);
    }
}