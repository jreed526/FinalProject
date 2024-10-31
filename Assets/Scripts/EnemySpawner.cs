using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    public GameObject enemyPrefab; // Reference to enemy prefab
    public Transform hero;         // Reference to hero's position
    public float spawnRadius = 15f; // Distance from the hero at which enemies will spawn
    public float spawnInterval = 2f; // Time between enemy spawns

    private void Start() {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies() {
        while (true) {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval); // Waits before spawning the next enemy
        }
    }

    private void SpawnEnemy() {
        // Choose a random point around the hero within the spawnRadius
        Vector2 randomPoint = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPosition = new Vector3(hero.position.x + randomPoint.x, hero.position.y, hero.position.z + randomPoint.y);

        // Instantiate the enemy at the chosen spawn position
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
