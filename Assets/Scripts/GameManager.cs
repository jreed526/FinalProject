using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TMP_Text roundText;         // TMP Text to display round info
    public TMP_Text timerText;         // TMP Text to display the round/grace period timer
    public int roundDuration = 120;    // Duration of each round in seconds
    public int gracePeriodDuration = 30; // Grace period duration in seconds

    // Enemy Prefabs
    public GameObject enemyType1Prefab;
    public GameObject enemyType2Prefab;
    public GameObject enemyType3Prefab;
    public GameObject enemyType4Prefab;
    public GameObject bossPrefab;

    public float spawnRadius = 15f;    // Spawn area radius around the hero
    public float enemySpawnInterval = 2f; // Initial spawn interval for enemies

    // Boss settings
    private bool isBossAlive = false;
    private float bossRespawnDelay = 15f;

    // Private variables
    private int currentRound = 1;
    private float roundTimer;
    private float gracePeriodTimer;
    private bool isGracePeriod = false;
    private bool isSpawningEnemies = true;
    private bool gameEnded = false;

    private Transform heroTransform;
    private PlayerController heroScript;
    private GameObject[] activeEnemies;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "_Start_Screen")
        {
            GameObject heroObject = GameObject.FindGameObjectWithTag("Hero");
            if (heroObject != null)
            {
                heroTransform = heroObject.transform;
                heroScript = heroObject.GetComponent<PlayerController>();
            }

            roundTimer = roundDuration;
            gracePeriodTimer = gracePeriodDuration;

            UpdateRoundUI();
            UpdateTimerUI(roundTimer);

            StartCoroutine(RoundCycle());
            StartCoroutine(EnemySpawning());
        }
    }

    private void Update()
    {
        if (!gameEnded && heroScript != null && currentRound <= 5)
        {
            CheckGameOverConditions();
        }
    }

    private IEnumerator RoundCycle()
    {
        while (currentRound <= 5)
        {
            if (!isGracePeriod)
            {
                roundTimer -= Time.deltaTime;
                UpdateTimerUI(roundTimer);

                if (roundTimer <= 0)
                {
                    isGracePeriod = true;
                    roundTimer = 0;
                    UpdateTimerUI(roundTimer);
                    StartGracePeriod();
                }
            }
            else
            {
                gracePeriodTimer -= Time.deltaTime;
                UpdateTimerUI(gracePeriodTimer);

                if (gracePeriodTimer <= 0)
                {
                    isGracePeriod = false;
                    gracePeriodTimer = gracePeriodDuration;
                    currentRound++;
                    roundTimer = roundDuration;
                    UpdateRoundUI();
                    UpdateTimerUI(roundTimer);

                    // Re-enable hero shooting
                    if (heroScript != null)
                    {
                        heroScript.SetShootingEnabled(true);
                    }

                    if (currentRound == 2)
                    {
                        IncreaseEnemy1Speed();
                    }
                    else if (currentRound == 3)
                    {
                        IncreaseEnemy2Speed();
                    }

                    RespawnEnemies();
                    yield return new WaitForSeconds(1);

                    if (currentRound == 5)
                    {
                        StartCoroutine(SpawnBoss());
                    }
                }
            }

            yield return null;
        }

        if (currentRound > 5 && !gameEnded)
        {
            EndGame();
        }
    }

    private void StartGracePeriod()
    {
        isSpawningEnemies = false;
        roundText.text = "Grace Period";
        DestroyAllEnemies();

        // Disable hero shooting
        if (heroScript != null)
        {
            heroScript.SetShootingEnabled(false);
        }
    }

    private void RespawnEnemies()
    {
        isSpawningEnemies = true;
    }

    private void DestroyAllEnemies()
    {
        // Destroy all enemies
        activeEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in activeEnemies)
        {
            Destroy(enemy);
        }

        // Destroy all enemy projectiles
        GameObject[] enemyProjectiles = GameObject.FindGameObjectsWithTag("EnemyProjectile");
        foreach (var projectile in enemyProjectiles)
        {
            Destroy(projectile);
        }
    }


    private void UpdateRoundUI()
    {
        roundText.text = "Round: " + currentRound;
    }

    private void UpdateTimerUI(float timeRemaining)
    {
        if (isGracePeriod)
        {
            timerText.text = "Grace: " + Mathf.Ceil(gracePeriodTimer).ToString();
        }
        else
        {
            timerText.text = "Time: " + Mathf.Ceil(timeRemaining).ToString();
        }
    }

    private IEnumerator EnemySpawning()
    {
        while (true)
        {
            if (!isGracePeriod && isSpawningEnemies)
            {
                SpawnEnemies();
            }

            yield return new WaitForSeconds(enemySpawnInterval);
        }
    }

    private void SpawnEnemies()
    {
        GameObject enemyToSpawn = null;

        if (currentRound == 1)
        {
            enemyToSpawn = enemyType1Prefab;
        }
        else if (currentRound == 2)
        {
            enemyToSpawn = Random.Range(0, 2) == 0 ? enemyType1Prefab : enemyType2Prefab;
            enemySpawnInterval = 1.8f;
        }
        else if (currentRound == 3)
        {
            enemyToSpawn = Random.Range(0, 3) == 0 ? enemyType3Prefab : enemyType2Prefab;
            enemySpawnInterval = (enemyToSpawn == enemyType3Prefab) ? 2.5f : 1.5f;
        }
        else if (currentRound == 4)
        {
            int enemyType = Random.Range(0, 10);
            if (enemyType < 4)
                enemyToSpawn = enemyType1Prefab;
            else if (enemyType < 8)
                enemyToSpawn = enemyType3Prefab;
            else
                enemyToSpawn = enemyType4Prefab;
            enemySpawnInterval = (enemyToSpawn == enemyType4Prefab) ? 3.0f : 2.0f;
        }
        else if (currentRound == 5)
        {
            if (!isBossAlive)
            {
                return;
            }
            
            int enemyType = Random.Range(0, 6);
            if (enemyType < 2)
                enemyToSpawn = enemyType2Prefab;
            else
                enemyToSpawn = enemyType3Prefab;
            enemySpawnInterval = (enemyToSpawn == enemyType3Prefab) ? 3.0f : 2.0f;
        }

        if (enemyToSpawn != null && heroTransform != null)
        {
            Vector2 randomPoint = Random.insideUnitCircle.normalized * spawnRadius;
            Vector3 spawnPosition = new Vector3(heroTransform.position.x + randomPoint.x, heroTransform.position.y, heroTransform.position.z + randomPoint.y);
            Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
        }
    }

    private IEnumerator SpawnBoss()
    {
        isBossAlive = true;
        SpawnBossEnemy();
        while (currentRound == 5)
        {
            if (!isBossAlive)
            {
                yield return new WaitForSeconds(bossRespawnDelay);
                SpawnBossEnemy();
            }
            yield return null;
        }
    }

    private void SpawnBossEnemy()
    {
        Vector3 spawnPosition = heroTransform.position + new Vector3(Random.Range(-spawnRadius, spawnRadius), 0, Random.Range(-spawnRadius, spawnRadius));
        Instantiate(bossPrefab, spawnPosition, Quaternion.identity);
        isBossAlive = true;
    }

    public void OnBossDefeated()
    {
        isBossAlive = false;
    }

    private void CheckGameOverConditions()
    {
        if (heroScript != null && heroScript.CurrentHealth <= 0)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        gameEnded = true;
        SceneManager.LoadScene("_Game_Over");
    }

    private void IncreaseEnemy1Speed()
    {
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Enemy1 enemy1Component = enemy.GetComponent<Enemy1>();
            if (enemy1Component != null)
            {
                enemy1Component.slimeSpeed *= 1.25f;
                enemy1Component.speed = enemy1Component.slimeSpeed;
            }
        }
    }

    private void IncreaseEnemy2Speed()
    {
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Enemy2 enemy2Component = enemy.GetComponent<Enemy2>();
            if (enemy2Component != null)
            {
                enemy2Component.speed = enemy2Component.standardSpeed;
            }
        }
    }
}