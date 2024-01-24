using UnityEngine;
using System.Collections.Generic;

namespace BaseEnemy
{
    public class WaveSpawner : MonoBehaviour
    {
        public WaveData waveData;
        public List<Transform> spawnPoints = new List<Transform>();
        private int currentWaveIndex = 0;
        private float nextSpawnTime;

        void Start()
        {
            nextSpawnTime = Time.time + waveData.spawnRateInSecs;
        }

        void Update()
        {
            if (currentWaveIndex < waveData.waveEnemies.Count && Time.time >= nextSpawnTime)
            {
                SpawnWave();
                currentWaveIndex++;

                if (currentWaveIndex < waveData.waveEnemies.Count)
                {
                    nextSpawnTime = Time.time + waveData.spawnRateInSecs;
                }
                else
                {
                    // Ondate completate
                }
            }
        }

        void SpawnWave()
        {
            if (currentWaveIndex < waveData.waveEnemies.Count)
            {
                WaveData.WaveEnemy currentWave = waveData.waveEnemies[currentWaveIndex];

                foreach (GameObject enemyPrefab in currentWave.enemyPrefab)
                {
                    Transform randomSpawnPoint = GetRandomSpawnPoint();

                    if (randomSpawnPoint != null)
                    {
                        SpawnEnemy(enemyPrefab, randomSpawnPoint);
                    }
                    else
                    {
                        Debug.LogError("Nessun punto di spawn disponibile!");
                        return;
                    }
                }
            }
        }

        void SpawnEnemy(GameObject enemyPrefab, Transform spawnPoint)
        {
            Vector3 spawnPosition = spawnPoint.position;

            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, this.transform);
        }

        Transform GetRandomSpawnPoint()
        {
            if (spawnPoints.Count > 0)
            {
                int randomIndex = Random.Range(0, spawnPoints.Count);
                Transform randomSpawnPoint = spawnPoints[randomIndex];

                return randomSpawnPoint;
            }

            return null;
        }
    }
}
