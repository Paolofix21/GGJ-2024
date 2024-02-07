using Code.EnemySystem.Wakakas;
using Code.Promises;
using UnityEngine;
using Utilities;

namespace Code.GameModeUtils.WaveBasedMode {
    [RequireComponent(typeof(WakakaBehaviour))]
    public class WaveBasedWakakaEntity : MonoBehaviour, IEntity {
        #region Private Variables
        private WakakaBehaviour _wakakaBehaviour;
        #endregion

        #region Behaviour Callbacks
        private void Awake() => _wakakaBehaviour = GetComponent<WakakaBehaviour>();

        private void OnDestroy() => OnDestroyed?.Invoke(this);
        #endregion

        #region IEntity
        public event DestroyEventHandler<IEntity> OnDestroyed;

        public Transform Transform => transform;

        public void Enable() => _wakakaBehaviour.enabled = true;

        public void Disable() => _wakakaBehaviour.enabled = false;
        #endregion
    }
}