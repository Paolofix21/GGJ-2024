using Code.Core.MatchManagers;
using Code.EnemySystem.Boss;
using Code.Promises;
using UnityEngine;

namespace Code.GameModeUtils.WaveBasedMode {
    [RequireComponent(typeof(WakakaBossBehaviour))]
    public class WaveBasedBossEntity : MonoBehaviour, IEntity {
        #region Public Variables
        #endregion

        #region Private Variables
        private WakakaBossBehaviour _controller;
        #endregion

        #region Properties
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            _controller = GetComponent<WakakaBossBehaviour>();
        }

        private void Start() {
            WaveBasedMatchManager.Singleton.SetBoss(this);
        }
        #endregion

        #region IEntity
        public Transform Transform => transform;

        public void Enable() {
            _controller.enabled = true;
        }

        public void Disable() {
            _controller.enabled = false;
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