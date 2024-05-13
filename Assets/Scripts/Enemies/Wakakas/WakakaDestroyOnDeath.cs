using UnityEngine;

namespace Code.EnemySystem.Wakakas {
    [RequireComponent(typeof(WakakaHealth))]
    public class WakakaDestroyOnDeath : MonoBehaviour {
        #region Behaviour Callbacks
        private void Awake() => GetComponent<WakakaHealth>().OnDeath += () => Destroy(gameObject);
        #endregion
    }
}