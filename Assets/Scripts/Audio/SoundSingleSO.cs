using UnityEngine;

namespace Audio {
    [CreateAssetMenu(menuName = "Audio/Sound", fileName = "New Sound")]
    public sealed class SoundSingleSO : SoundSO {
        #region Public Variables
        [SerializeField] private Sound m_sound;
        #endregion

        #region Overrides
        public override Sound GetSound() => m_sound;
        #endregion
    }
}