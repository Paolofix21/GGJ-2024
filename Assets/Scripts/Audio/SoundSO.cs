using UnityEngine;

namespace Audio {
    public abstract class SoundSO : ScriptableObject {
        #region Public Methods
        public abstract Sound GetSound();
        #endregion
    }
}