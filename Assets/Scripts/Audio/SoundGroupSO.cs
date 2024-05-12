using System.Collections.Generic;
using UnityEngine;

namespace Audio {
    [CreateAssetMenu(menuName = "Audio/Sound Group", fileName = "New Sound Gruop")]
    public sealed class SoundGroupSO : SoundSO {
        #region Types
        private enum ExtractionMethod {
            Sequence,
            Random,
            RandomWithoutRepetition
        }
        #endregion

        #region Public Variables
        [SerializeField] private ExtractionMethod m_method = ExtractionMethod.Sequence;
        [SerializeField] private List<Sound> m_sounds = new();
        #endregion

        #region Private Variables
        private int _index = 0;
        private List<int> _takes, _taken;
        #endregion

        #region Overrides
        public override Sound GetSound() {
            if (_takes == null || _taken.Count <= 0) {
                _takes = new();
                _taken = new();

                for (var i = 0; i < m_sounds.Count; i++)
                    _takes.Add(i);
            }

            switch (m_method) {
                case ExtractionMethod.Sequence:
                    _index = ++_index % m_sounds.Count;
                    return m_sounds[_index];
                case ExtractionMethod.Random:
                    return m_sounds[Random.Range(0, m_sounds.Count)];
                case ExtractionMethod.RandomWithoutRepetition:
                    var at = Random.Range(0, _takes.Count);
                    _index = _takes[at];
                    _takes.RemoveAt(at);
                    _taken.Add(_index);

                    if (_takes.Count > 0)
                        return m_sounds[_index];

                    _takes.AddRange(_taken);
                    _taken.Clear();
                    return m_sounds[_index];
                default:
                    return null;
            }
        }
        #endregion
    }
}