using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour {
    public TMP_Text roundText;         // TMP Text to display round info
    public TMP_Text timerText;         // TMP Text to display the round/grace period timer
    public int roundDuration = 120;    // Duration of each round in seconds
    public int gracePeriodDuration = 30; // Grace period duration in seconds

    // Enemy Prefabs
    public GameObject enemyType1Prefab;
    public GameObject enemyType2Prefab;
    public GameObject enemyType3Prefab;
    public GameObject bossPrefab;

    public float spawnRadius = 15f;   // Spawn area radius around the hero
    public float enemySpawnInterval = 2f; // Initial spawn interval for enemies

    // Private variables
    private int currentRound = 1;
    private float roundTimer;
    private float gracePeriodTimer;
    private bool isGracePeriod = false;
    private bool isSpawningEnemies = true;

    private Transform hero;  // Reference to hero's position (can be set in the Inspector)
    private GameObject[] activeEnemies;  // Store active enemies for respawn after grace period

    private void Start() {
        hero = GameObject.FindGameObjectWithTag("Hero").transform; // Find the hero by its tag

        roundTimer = roundDuration;
        gracePeriodTimer = gracePeriodDuration;

        // Update the UI initially
        UpdateRoundUI();
        UpdateTimerUI(roundTimer);
        
        // Start the round cycle and enemy spawning
        StartCoroutine(RoundCycle());
        StartCoroutine(EnemySpawning());
    }

    private IEnumerator RoundCycle() {
        while (true) {
            if (!isGracePeriod) {
                // Round Timer countdown
                roundTimer -= Time.deltaTime;
                UpdateTimerUI(roundTimer);

                if (roundTimer <= 0) {
                    // End of round, start grace period
                    isGracePeriod = true;
                    roundTimer = 0;
                    UpdateTimerUI(roundTimer);
                    StartGracePeriod();
                }
            } else {
                // Grace Period countdown
                gracePeriodTimer -= Time.deltaTime;
                UpdateTimerUI(gracePeriodTimer);

                if (gracePeriodTimer <= 0) {
                    // Grace period ends, start new round
                    isGracePeriod = false;
                    gracePeriodTimer = gracePeriodDuration;
                    currentRound++;
                    roundTimer = roundDuration;
                    UpdateRoundUI();
                    UpdateTimerUI(roundTimer);
                    RespawnEnemies(); // Respawn the enemies after grace period
                    yield return new WaitForSeconds(1);  // Pause before the next round starts
                }
            }

            yield return null;
        }
    }

    private void StartGracePeriod() {
        // Disable enemy spawning during grace period
        isSpawningEnemies = false;
        roundText.text = "Grace Period";  // Update round text to indicate grace period

        // Destroy all enemies at the end of the round (but don't drop XP)
        DestroyAllEnemies();
    }

    private void RespawnEnemies() {
        // Respawn enemies after the grace period
        StartCoroutine(EnemySpawning()); // Restart enemy spawning after grace period ends
    }

    private void DestroyAllEnemies() {
        // Find all active enemies and destroy them
        activeEnemies = GameObject.FindGameObjectsWithTag("Enemy"); // Store all currently active enemies
        foreach (var enemy in activeEnemies) {
            Destroy(enemy); // Destroy all active enemies on screen
        }
    }

    private void UpdateRoundUI() {
        roundText.text = "Round: " + currentRound;  // Update round UI with current round number
    }

    private void UpdateTimerUI(float timeRemaining) {
        if (isGracePeriod) {
            // Display grace period timer
            timerText.text = "Grace: " + Mathf.Ceil(gracePeriodTimer).ToString();
        } else {
            // Display round timer
            timerText.text = "Time: " + Mathf.Ceil(timeRemaining).ToString();
        }
    }

    private IEnumerator EnemySpawning() {
        while (true) {
            if (!isGracePeriod && isSpawningEnemies) {
                SpawnEnemies();
            }

            yield return new WaitForSeconds(enemySpawnInterval);
        }
    }

    private void SpawnEnemies() {
        // Spawn enemies based on the current round
        GameObject enemyToSpawn = null;

        if (currentRound == 1) {
            // Round 1: Simple enemies, spawn one of enemy type 1
            enemyToSpawn = enemyType1Prefab;
        } else if (currentRound == 2) {
            // Round 2: Slightly faster enemyType1, introduce enemyType2
            enemyToSpawn = Random.Range(0, 2) == 0 ? enemyType1Prefab : enemyType2Prefab;
            enemySpawnInterval = 1.8f;  // Slightly faster spawn interval
        } else if (currentRound == 3) {
            // Round 3: EnemyType2 speed is increased
            enemyToSpawn = enemyType2Prefab;
            enemySpawnInterval = 1.5f;  // Standard spawn rate
        } else if (currentRound == 4) {
            // Round 4: Introduce enemyType3
            enemyToSpawn = Random.Range(0, 2) == 0 ? enemyType1Prefab : enemyType3Prefab;
        } else if (currentRound == 5) {
            // Round 5: Introduce boss and adjust spawn rates
            if (Random.Range(0, 10) < 1) { // 10% chance to spawn boss
                enemyToSpawn = bossPrefab;
            } else {
                enemyToSpawn = Random.Range(0, 2) == 0 ? enemyType2Prefab : enemyType3Prefab;
            }
            enemySpawnInterval = 2f;  // Slower spawn for normal enemies
        }

        // Spawn the selected enemy
        if (enemyToSpawn != null) {
            Vector2 randomPoint = Random.insideUnitCircle.normalized * spawnRadius;
            Vector3 spawnPosition = new Vector3(hero.position.x + randomPoint.x, hero.position.y, hero.position.z + randomPoint.y);
            Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
        }

        // End round logic (despawn enemies and don't drop XP)
        if (roundTimer <= 0) {
            // Destroy all enemies without dropping XP
            foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy")) {
                Destroy(enemy); // Or handle despawning logic here
            }
        }
    }
}
