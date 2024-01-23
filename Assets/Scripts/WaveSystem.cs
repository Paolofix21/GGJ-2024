using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    public GameObject enemyPrefab;    // Il prefab del nemico da spawnare
    public Transform spawnArea;       // L'area in cui spawnare i nemici
    public int numberOfEnemies = 5;   // Il numero di nemici da spawnare
    public float spawnRate = 2f;      // Il tempo tra gli spawn dei nemici in secondi

    void Start()
    {
        InvokeRepeating("SpawnEnemies", 0f, spawnRate);
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        // Genera coordinate casuali all'interno dell'area di spawn
        float randomX = Random.Range(spawnArea.position.x - spawnArea.localScale.x / 2f, spawnArea.position.x + spawnArea.localScale.x / 2f);
        float randomZ = Random.Range(spawnArea.position.z - spawnArea.localScale.z / 2f, spawnArea.position.z + spawnArea.localScale.z / 2f);

        Vector3 randomSpawnPoint = new Vector3(randomX, spawnArea.position.y, randomZ);

        Instantiate(enemyPrefab, randomSpawnPoint, Quaternion.identity);
    }
}
