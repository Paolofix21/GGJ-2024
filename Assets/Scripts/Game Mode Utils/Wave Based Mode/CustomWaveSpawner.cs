using Code.Core;
using Code.Core.MatchManagers;
using UnityEngine;

namespace Code.GameModeUtils.WaveBasedMode {
    public class CustomWaveSpawner : MonoBehaviour {
        #region Public Variables
        [SerializeField] private MinorWaveInfoSO m_wave;
        #endregion

        #region Public Methods
        public void SpawnIfTrue(bool value) {
            if (!value)
                return;

            Debug.Log("Spawn custom wave...\n");
            Spawn();
        }

        public void Spawn() => GameEvents.GetMatchManager<WaveBasedMatchManager>().EntityManager.SpawnWaveCustom(m_wave);
        #endregion
    }
}