using Code.Core.MatchManagers;
using Code.EnemySystem.Boss;
using Code.Promises;
using UnityEngine;
using Utilities;

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

            Disable();
        }

        private void Start() => WaveBasedMatchManager.Singleton.SetBoss(this);

        private void OnDestroy() => OnDestroyed?.Invoke(this);
        #endregion

        #region IEntity
        public event DestroyEventHandler<IEntity> OnDestroyed;

        public Transform Transform => transform;

        public void Enable() {
            _controller.enabled = true;
        }

        public void Disable() {
            _controller.enabled = false;
        }
        #endregion

        #region Public Methods
        public void StartFight() => _controller.BeginFight();
        #endregion

        #region Private Methods
        #endregion

        #region Event Methods
        #endregion
    }
}