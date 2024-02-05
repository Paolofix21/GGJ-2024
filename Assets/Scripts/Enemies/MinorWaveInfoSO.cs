using System.Collections.Generic;
using Code.GameModeUtils.WaveBasedMode;
using UnityEngine;

namespace Code.EnemySystem {
    [CreateAssetMenu(menuName = "Waves/Wave", fileName = "New Wave")]
    public sealed class MinorWaveInfoSO : ScriptableObject {
        #region Public Variables
        [field: SerializeField] public float SpawnDelay { get; private set; } = .25f;
        [SerializeField] public bool m_randomExtraction = true;
        [SerializeField] private List<WaveBasedWakakaEntity> m_entities = new();
        #endregion

        #region Private Variables
        private readonly List<int> _remainingIndices = new();
        #endregion

        #region Public Methods
        public void Init() {
            _remainingIndices.Clear();

            for (var i = 0; i < m_entities.Count; i++)
                _remainingIndices.Add(i);
        }

        public bool TryExtraction(out WaveBasedWakakaEntity entity) {
            if (_remainingIndices.Count <= 0) {
                entity = null;
                return false;
            }

            var extractIndex = m_randomExtraction ? Random.Range(0, _remainingIndices.Count) : 0;
            var index = _remainingIndices[extractIndex];
            entity = m_entities[index];

            _remainingIndices.RemoveAt(extractIndex);
            return true;
        }
        #endregion
    }
}