using System.Collections.Generic;
using UnityEngine;

namespace Code.GameModeUtils.WaveBasedMode {
    [System.Serializable]
    public sealed class MajorWaveInfo {
        #region Public Variables
        [field: SerializeField] public float Delay { get; private set; } = 1f;
        [SerializeField] private List<MinorWaveInfoSO> m_waves = new();
        #endregion

        #region Private Variables
        private int _index = -1;
        #endregion

        #region Public Methods
        public void Init() {
            _index = -1;
            m_waves.ForEach(w => w.Init());
        }

        public bool TryGetNextSubWave(out MinorWaveInfoSO minorWaveInfo) {
            ++_index;

            if (_index >= m_waves.Count) {
                minorWaveInfo = null;
                return false;
            }

            minorWaveInfo = m_waves[_index];
            return true;
        }

        public int GetRemainingWavesCount() => m_waves.Count - _index - 1;
        #endregion
    }
}