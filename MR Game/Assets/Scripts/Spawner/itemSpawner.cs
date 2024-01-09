using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class itemSpawner : MonoBehaviour
{
    public GameObject[] ItemPrefabs;
    public Vector3 SpawnArea = new Vector3(10, 0, 10);
    public float MinSpawnTime = 30;
    public float MaxSpawnTime = 60;

    private void Start()
    {
        StartCoroutine(SpawnItem());
    }

    private IEnumerator SpawnItem()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(MinSpawnTime, MaxSpawnTime));

            Vector3 spawnPosition = new Vector3(
                Random.Range(-SpawnArea.x, SpawnArea.x),
                SpawnArea.y,
                Random.Range(-SpawnArea.z, SpawnArea.z)
            );

            int ItemIndex = Random.Range(0, ItemPrefabs.Length);
            Instantiate(ItemPrefabs[ItemIndex], SpawnArea, Quaternion.identity);
        }
    }
}
