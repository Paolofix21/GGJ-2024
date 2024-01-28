using System.Collections.Generic;
using UnityEngine;

namespace Code.EnemySystem
{
    public class WaveSpawner : MonoBehaviour
    {
        public List<Transform> spawnPoints = new List<Transform>();
        public List<WaveData> waveData = new List<WaveData>();
        [HideInInspector]public float enemyToKill = 0;
        private int currentInternalWaveIndex = 0;
        private int waveNumber;


        void Update()
        {
#if UNITY_EDITOR // only for internal test
            if (Input.GetKeyDown(KeyCode.M))
            {
                waveNumber++;
                currentInternalWaveIndex = 0;
            }
#endif
            
            if (enemyToKill <= 0 && currentInternalWaveIndex < waveData[waveNumber].waveEnemies.Count)
            {
                if (currentInternalWaveIndex >= waveData[waveNumber].waveEnemies.Count)
                {
                    // Ondata completata, possiamo chiamare funzione spawn boss
                    Debug.Log("Ondata completata");
                }

                else
                {
                    SpawnWave();
                    
                }

                
            }
        }

        /// <summary>
        /// Funzione da chiamare conclusa l'ondata
        /// </summary>
        public void SpawnNextWave()
        {
            waveNumber++;
            currentInternalWaveIndex = 0;
        }



        void SpawnWave()
        {
            if (currentInternalWaveIndex < waveData[waveNumber].waveEnemies.Count)
            {
                enemyToKill = waveData[waveNumber].waveEnemies[currentInternalWaveIndex].enemyPrefab.Count;

                WaveData.WaveEnemy currentWave = waveData[waveNumber].waveEnemies[currentInternalWaveIndex];

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

            AudioManager.instance.PlayOneShot(FMODEvents.instance.spawnEvent, spawnPosition);
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
