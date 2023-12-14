using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    private Transform playerTransform;
    private Transform enemiesParent;
    public GameObject[] enemyPrefabs;
    public float maxSpawnDistance = 20f;
    public float minEnemyDistance = 5f;
    public int initialEnemyCount = 5;
    public int roundDuration = 60; // In seconds

    private int currentRound = 1;
    private int score = 0;
    private float roundTimer;
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    public bool isGamePaused;
    public UIFillAmount uiFill;

    void Start()
    {
        // Find the OVRPlayer in the scene
        GameObject ovrPlayerGameObject = GameObject.Find("OVRPlayer");

        if (ovrPlayerGameObject != null)
        {
            playerTransform = ovrPlayerGameObject.transform;
        }
        else
        {
            Debug.LogError("OVRPlayer not found in the scene.");
        }

        // Find or create the 'Enemies' parent object
        GameObject enemiesParentObj = GameObject.Find("Enemies");
        if (enemiesParentObj == null)
        {
            enemiesParentObj = new GameObject("Enemies");
        }
        enemiesParent = enemiesParentObj.transform;
        StartRound();
    }

    void Update()
    {
        roundTimer -= Time.deltaTime;
        if (roundTimer <= 0 && spawnedEnemies.Count > 0)
        {
            GameOver();
        }

        if (spawnedEnemies.Count == 0 && roundTimer > 0)
        {
            StartNextRound();
        }

        if (Input.GetKeyDown(KeyCode.Escape) || OVRInput.GetDown(OVRInput.Button.Two))
        {
            if (isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        //pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isGamePaused = true;
    }

    public void ResumeGame()
    {
        //pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;
    }

    void SpawnEnemiesAroundPlayer(int enemyCount)
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPoint;
            do
            {
                Vector3 spawnDirection = Random.insideUnitSphere;
                spawnDirection.y = 0;
                spawnPoint = playerTransform.position + spawnDirection.normalized * Random.Range(minEnemyDistance, maxSpawnDistance);
            }
            while (!ValidSpawnPoint(spawnPoint));

            // Find a point on the NavMesh near the calculated spawnPoint
            if (NavMesh.SamplePosition(spawnPoint, out NavMeshHit hit, maxSpawnDistance, NavMesh.AllAreas))
            {
                GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
                GameObject enemy = Instantiate(enemyPrefab, hit.position, Quaternion.identity, enemiesParent);

                // Set the GameManager reference in the enemy's script
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.SetGameManager(this);
                }

                spawnedEnemies.Add(enemy);
            }
        }
    }

    bool ValidSpawnPoint(Vector3 point)
    {
        foreach (var enemy in spawnedEnemies)
        {
            if (Vector3.Distance(point, enemy.transform.position) < minEnemyDistance)
                return false;
        }
        return true;
    }

    public void EnemyKilled(GameObject enemy)
    {
        spawnedEnemies.Remove(enemy);
        score += 10; // Increase score
        uiFill.scoreEdit(score);
    }

    void StartRound()
    {
        SpawnEnemiesAroundPlayer(initialEnemyCount + currentRound - 1);
        roundTimer = roundDuration;
    }

    void StartNextRound()
    {
        currentRound++;
        StartRound();
    }

    void GameOver()
    {
        Debug.Log("Game over. Restarting in 10 seconds...");

        // Cancel any pending invocations to prevent double calls
        CancelInvoke(nameof(StartRound));
        CancelInvoke(nameof(StartNextRound));

        // Destroy all active enemies
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        spawnedEnemies.Clear();
        score = 0;
        currentRound = 1;



        uiFill.scoreEdit(score);

        // Restart the round after a delay
        Invoke(nameof(StartRound), 10);
    }
}
