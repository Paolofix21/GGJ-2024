using Code.Core.MatchManagers;
using Code.EnemySystem;
using Code.Promises;
using UnityEngine;

namespace Code.GameModeUtils.WaveBasedMode {
    [RequireComponent(typeof(WakakaBehaviour))]
    [RequireComponent(typeof(WakakaHealth))]
    public class WaveBasedWakakaEntity : MonoBehaviour, IEntity {
        #region Public Variables
        #endregion

        #region Private Variables
        private WakakaBehaviour _wakakaBehaviour;
        private WakakaHealth _wakakaHealth;
        #endregion

        #region Properties
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            _wakakaBehaviour = GetComponent<WakakaBehaviour>();
            _wakakaHealth = GetComponent<WakakaHealth>();
        }

        private void Start() => WaveBasedMatchManager.Singleton.EntityManager.Entities.Add(this);
        private void OnDestroy() => WaveBasedMatchManager.Singleton.EntityManager.Entities.Remove(this);
        #endregion

        #region IEntity
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

        #region Public Methods
        #endregion

        #region Private Methods
        #endregion

        #region Event Methods
        #endregion
    }
}