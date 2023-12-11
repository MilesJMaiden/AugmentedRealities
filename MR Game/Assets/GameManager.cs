using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    private Transform playerTransform;
    public GameObject[] enemyPrefabs;
    public float maxSpawnDistance = 20f;
    public float minEnemyDistance = 5f;
    public int initialEnemyCount = 5;
    public int roundDuration = 60; // In seconds

    private int currentRound = 1;
    private int score = 0;
    private float roundTimer;
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    void Awake()
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
    }

    void Start()
    {
        StartRound();
    }

    void Update()
    {
        roundTimer -= Time.deltaTime;
        if (roundTimer <= 0)
        {
            GameOver();
        }

        if (spawnedEnemies.Count == 0)
        {
            StartNextRound();
        }
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

            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);
            spawnedEnemies.Add(enemy);
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
        score = 0;
        currentRound = 1;
        // Restart logic goes here...
        Invoke(nameof(StartRound), 10);
    }
}