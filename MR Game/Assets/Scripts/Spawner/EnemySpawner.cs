using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject EnemyPrefab; // enemy prefabs
    public float IntervalTime = 5; // interval time
    public Vector3 SpawnArea = new Vector3(10, 0, 10); // spaces
    public float HealthIncrement = 10; // health increment
    public float DamageIncrement = 5; // damage increment
    public GameObject Player; //player
    public PlayerAttributes PlayerAttributes;//health controller


    private int EnemyCounter; // enemy number
    private int EnemiesNumber = 15; // Number of enemies to spawn every time

    void Start()
    {
        if (Player == null)
        {
            Debug.LogError("Player object not found.");
            return;
        }
        EnemyCounter = 0;
        

        if (PlayerAttributes == null)
        {
            PlayerAttributes = Player.GetComponent<PlayerAttributes>();
            if (PlayerAttributes == null)
            {
                Debug.LogError("PlayerAttributes script not found.");
                return;
            }
        }

        EnemyCounter = 0;
        for (int i = 0; i < EnemiesNumber; i++)
        {
            CreateEnemy();
        }
        InvokeRepeating("CreateEnemy", IntervalTime, IntervalTime);
    }
        
    void Update()
    {

    }

    public class EnemyAttributes : MonoBehaviour
    {
        public float EnemyHp;
        public float EnemyDamage;
    }

    public void CreateEnemy()
    {
        // need to change the name of currentHp
        if (PlayerAttributes.health > 0 && EnemyCounter < EnemiesNumber)
        {
            Vector3 spawnPosition = new Vector3(
                Random.Range(-SpawnArea.x, SpawnArea.x),
                SpawnArea.y,
                Random.Range(-SpawnArea.z, SpawnArea.z)
            );

            GameObject newEnemy = Instantiate(EnemyPrefab, spawnPosition, Quaternion.identity);

            newEnemy.GetComponent<EnemyAttributes>().EnemyHp += HealthIncrement;
            newEnemy.GetComponent<EnemyAttributes>().EnemyDamage += DamageIncrement;

            EnemyCounter++;
        }

        else
        {
            CancelInvoke();
        }
    }

    
}