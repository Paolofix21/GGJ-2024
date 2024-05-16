using UnityEngine;

namespace Code.EnemySystem.Wakakas {
    [RequireComponent(typeof(WakakaHealth))]
    public class WakakaDestroyOnDeath : MonoBehaviour {
        #region Public Variables
        public event System.Action<WakakaDestroyOnDeath> OnTerminate;
        #endregion

        #region Behaviour Callbacks
        private void Awake() => GetComponent<WakakaHealth>().OnDeath += () => Destroy(gameObject);

        private void OnDestroy() {
            OnTerminate?.Invoke(this);
            OnTerminate = null;
        }
        #endregion
    }
}