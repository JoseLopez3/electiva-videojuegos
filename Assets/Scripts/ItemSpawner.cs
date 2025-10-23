using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] itemPrefabs; // Arrastra aquí tus prefabs de poción y estrella
    public float spawnInterval = 10f; // Tiempo entre cada aparición
    public Transform[] spawnPoints; // Puntos donde pueden aparecer los items

    void Start()
    {
        StartCoroutine(SpawnItemsRoutine());
    }

    private IEnumerator SpawnItemsRoutine()
    {
        while (true) // Bucle infinito
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnRandomItem();
        }
    }

    void SpawnRandomItem()
    {
        if (itemPrefabs.Length == 0 || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No hay prefabs o puntos de spawn asignados.");
            return;
        }

        // Elige un item y un punto de spawn al azar
        int randomItemIndex = Random.Range(0, itemPrefabs.Length);
        int randomSpawnIndex = Random.Range(0, spawnPoints.Length);

        GameObject itemToSpawn = itemPrefabs[randomItemIndex];
        Transform spawnPoint = spawnPoints[randomSpawnIndex];

        Instantiate(itemToSpawn, spawnPoint.position, Quaternion.identity);
        Debug.Log("Ha aparecido un " + itemToSpawn.name);
    }
}