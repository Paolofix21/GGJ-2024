using UnityEngine;

namespace Code.EnemySystem.Wakakas {
    [RequireComponent(typeof(WakakaHealth))]
    public class WakakaDestroyOnDeath : MonoBehaviour {
        #region Public Variables
        public event System.Action<WakakaDestroyOnDeath> OnTerminate;
        #endregion

        #region Behaviour Callbacks
        private void Awake() => GetComponent<WakakaHealth>().OnDeath += Terminate;

        private void OnDestroy() => OnTerminate = null;
        #endregion

        #region Private Methods
        private void Terminate() {
            OnTerminate?.Invoke(this);
            Destroy(gameObject);
        }
        #endregion
    }
}