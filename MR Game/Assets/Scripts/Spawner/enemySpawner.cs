using UnityEngine;

public class enemySpawner : MonoBehaviour
{
    public GameObject[] EnemyPrefabs; // enemy prefabs
    public float IntervalTime = 15; // interval time
    public Vector3 SpawnArea = new Vector3(10, 0, 10); // spaces
    public int HealthIncrement = 10; // health increment
    public int DamageIncrement = 5; // damage increment
    public PlayerAttributes PlayerAttributes;//player health controller
    public EnemyHealth enemyHealth; // enemy health controller

    public int enemyCounter; // enemy number
    public int EnemiesNumber = 3; // Number of enemies to spawn every time

    void Start()
    {
        enemyCounter = 0;
        if (PlayerAttributes == null)
        {
            PlayerAttributes = GetComponent<PlayerAttributes>();
            if (PlayerAttributes == null)
            {
                Debug.LogError("PlayerAttributes script not found.");
                return;
            }
        }

        for (int i = 0; i < EnemiesNumber; i++)
        {
            createEnemy();
        }
        InvokeRepeating("CreateEnemy", IntervalTime, IntervalTime);
    }


    public void createEnemy()
    {

        // need to change the name of currentHp
        if (PlayerAttributes.health > 0 && enemyCounter < EnemiesNumber)
        {
            Vector3 spawnPosition = new Vector3(
                Random.Range(-SpawnArea.x, SpawnArea.x),
                SpawnArea.y,
                Random.Range(-SpawnArea.z, SpawnArea.z)
            );

            int index = Random.Range(0, EnemyPrefabs.Length);
            GameObject newEnemy = Instantiate(EnemyPrefabs[index], spawnPosition, Quaternion.identity);

            EnemyHealth enemyHealth = newEnemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.enemyHp += HealthIncrement;
                enemyHealth.enemyDamage += DamageIncrement;
            }

            enemyCounter++;
        }

        else
        {
            CancelInvoke();
        }
    }


}
