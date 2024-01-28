using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "WaveData", menuName = "ScriptableObjects/Wave Data", order = 1)]
public class WaveData : ScriptableObject
{
    [Header("General Settings")]
    public float spawnRateInSecs;
    //public int numberOfWaves;

    [Header("Enemies 4 Wave")]
    public List<WaveEnemy> waveEnemies = new List<WaveEnemy>();

    public int SubWavesCount => waveEnemies.Count;

    [Serializable]
    public class WaveEnemy
    {
        public List<GameObject> enemyPrefab = new List<GameObject>();
    }
}
