using UnityEngine;
using System.Collections.Generic;

namespace Code.EnemySystem {
    [CreateAssetMenu(menuName = "Waves/Waves Collection", fileName = "New Waves Collection")]
    public class WavesCollectionSO : ScriptableObject {
        #region Public Variables
        [field: SerializeField] private List<MajorWaveInfo> m_majorWaves = new();
        #endregion

        #region Private Variables
        private int _index = -1;
        #endregion

        #region Public Methods
        public void Init() {
            _index = -1;
            m_majorWaves.ForEach(w => w.Init());
        }

        public bool TryGetNextWave(out MajorWaveInfo majorWaveInfo) {
            ++_index;

            if (_index >= m_majorWaves.Count) {
                majorWaveInfo = null;
                return false;
            }

            majorWaveInfo = m_majorWaves[_index];
            return true;
        }
        #endregion
    }
}
