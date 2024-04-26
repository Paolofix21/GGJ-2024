using Audio;
using UnityEngine;

namespace Player {
    public class CameraListenerHelper : MonoBehaviour {
        #region Behaviour Callbacks
        private void Awake() => AudioManager.Singleton.SetListenerState(false);

        private void OnDestroy() {
            if (AudioManager.Singleton)
                AudioManager.Singleton.SetListenerState(true);
        }
        #endregion
    }
}