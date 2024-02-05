using Code.EnemySystem;
using Code.Promises;
using UnityEngine;
using Utilities;

namespace Code.GameModeUtils.WaveBasedMode {
    [RequireComponent(typeof(WakakaBehaviour))]
    [RequireComponent(typeof(WakakaHealth))]
    public class WaveBasedWakakaEntity : MonoBehaviour, IEntity {
        #region Private Variables
        private WakakaBehaviour _wakakaBehaviour;
        private WakakaHealth _wakakaHealth;
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            _wakakaBehaviour = GetComponent<WakakaBehaviour>();
            _wakakaHealth = GetComponent<WakakaHealth>();
        }

        private void OnDestroy() => OnDestroyed?.Invoke(this);
        #endregion

        #region IEntity
        public event DestroyEventHandler<IEntity> OnDestroyed;

        public Transform Transform => transform;

        public void Enable() {
            _wakakaBehaviour.enabled = true;
            _wakakaHealth.enabled = true;
        }

        public void Disable() {
            _wakakaBehaviour.enabled = false;
            _wakakaHealth.enabled = false;
        }
        #endregion
    }
}