using Code.EnemySystem.Boss;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.EnemySystem {
    public class WaveSpawner : MonoBehaviour {
        public List<Transform> spawnPoints = new List<Transform>();
        public GameObject Boss;
        public List<WaveData> waveData = new List<WaveData>();
        [HideInInspector] public float enemyToKill = 0;
        [SerializeField] private float waveInterval = 1f;

        private int currentInternalWaveIndex = 0;
        private int waveNumber = 0;

        public static event System.Action<int> OnMacroWaveIndexChanged;
        public static event System.Action OnBossFightStart;
        public static event System.Action OnEnemyDeath;
        private List<WakakaBehaviour> _currentEnemies = new();
        private float _nextWaveTime;

        private void Start() {
            OnMacroWaveIndexChanged?.Invoke(waveNumber);
        }

        void Update() {
#if UNITY_EDITOR // only for internal test
            if (Input.GetKeyDown(KeyCode.M)) {
                SpawnNextWave();
            }
#endif

            /*if (waveNumber < waveData.Count // se ci sono ancora ondate macro
                && _currentEnemies.Count <= 0 // sono finiti i nemici dell'ondata corrente
                && currentInternalWaveIndex < waveData[waveNumber].SubWavesCount) // ci sono altre sotto ondate
            {
                SpawnWave();
            }*/

            if (_currentEnemies.Any())
                return;

            if (currentInternalWaveIndex < waveData[waveNumber].SubWavesCount) // ci sono altre sotto ondate?
                SpawnSubWave();
            else if (_currentEnemies.Count <= 0) // ci sono altre macro ondate?
            {
                if (Time.time >= _nextWaveTime)
                    SpawnNextWave();
            }
        }

        private void OnDestroy() => OnMacroWaveIndexChanged?.Invoke(0);

        /// <summary>
        /// Funzione da chiamare conclusa l'ondata
        /// </summary>
        public void SpawnNextWave() {
            waveNumber++;
            currentInternalWaveIndex = 0;
            Debug.Log("Macro Ondata completata\n", this);
            OnMacroWaveIndexChanged?.Invoke(waveNumber);

            if (waveNumber < waveData.Count)
                return;

            Debug.Log("All waves completed\n", this);
            Boss.GetComponent<BossBehaviour>().StartPhase();
            enabled = false;
            OnBossFightStart?.Invoke();
        }

        void SpawnSubWave() {
            if (currentInternalWaveIndex < waveData[waveNumber].waveEnemies.Count) {
                enemyToKill = waveData[waveNumber].waveEnemies[currentInternalWaveIndex].enemyPrefab.Count;

                WaveData.WaveEnemy currentWave = waveData[waveNumber].waveEnemies[currentInternalWaveIndex];

                foreach (GameObject enemyPrefab in currentWave.enemyPrefab) {
                    Transform randomSpawnPoint = GetRandomSpawnPoint();

                    if (randomSpawnPoint != null) {
                        SpawnEnemy(enemyPrefab, randomSpawnPoint);
                    }
                    else {
                        Debug.LogError("Nessun punto di spawn disponibile!");
                        return;
                    }
                }

                currentInternalWaveIndex++;

                if (currentInternalWaveIndex >= waveData[waveNumber].waveEnemies.Count) {
                    // Ondata completata, possiamo chiamare funzione spawn boss
                    Debug.Log("Mini Ondata completata\n", this);
                    SpawnNextWave();
                }
            }
        }

        void SpawnEnemy(GameObject enemyPrefab, Transform spawnPoint) {
            Vector3 spawnPosition = spawnPoint.position;

            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, this.transform);

            AudioManager.instance.PlayOneShot(FMODEvents.instance.spawnEvent, spawnPosition);

            var enemyBehavior = enemy.GetComponent<WakakaBehaviour>();
            enemyBehavior.OnDeath += RemoveEnemy;
            _currentEnemies.Add(enemyBehavior);
        }

        private void RemoveEnemy(WakakaBehaviour enemyBehavior) {
            _currentEnemies.Remove(enemyBehavior);
            OnEnemyDeath.Invoke();
            if (_currentEnemies.Any())
                return;

            _nextWaveTime = Time.time + waveInterval;
        }

        Transform GetRandomSpawnPoint() {
            if (spawnPoints.Count > 0) {
                int randomIndex = Random.Range(0, spawnPoints.Count);
                Transform randomSpawnPoint = spawnPoints[randomIndex];

                return randomSpawnPoint;
            }

            return null;
        }
    }
}
