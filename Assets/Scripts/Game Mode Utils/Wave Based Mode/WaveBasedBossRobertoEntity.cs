using Code.Core.MatchManagers;
using Code.Promises;
using Enemies.BossRoberto;
using UnityEngine;
using Utilities;

namespace Code.GameModeUtils.WaveBasedMode {
    [RequireComponent(typeof(WakakaBossRobertoBehaviour))]
    public class WaveBasedBossRobertoEntity : MonoBehaviour, IEntity {
        #region Public Variables
        public event System.Action OnSurrender;
        #endregion

        #region Private Variables
        private WakakaBossRobertoBehaviour _controller;
        #endregion

        #region Properties
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            _controller = GetComponent<WakakaBossRobertoBehaviour>();
            _controller.OnSurrender += Surrender;

            Disable();
        }

        // private void Start() => StartFight();
        private void Update() {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F3) && _controller.Phase == WakakaBossRobertoBehaviour.WakakaBossState.None) {
                Enable();
                StartFight();
            }
            if (Input.GetKeyDown(KeyCode.F4) && _controller.Phase != WakakaBossRobertoBehaviour.WakakaBossState.None) {
                _controller.Health.ApplyDamage(Mathf.Infinity, _controller.gameObject);
            }
#endif
        }

        private void OnDestroy() => OnDestroyed?.Invoke(this);
        #endregion

        #region IEntity
        public event DestroyEventHandler<IEntity> OnDestroyed;

        public Transform Transform => transform;

        public void Enable() => _controller.Enabled = true;

        public void Disable() => _controller.Enabled = false;

        public void Terminate() { }
        #endregion

        #region Public Methods
        [ContextMenu("Fight")]
        public void StartFight() => _controller.BeginFight(WaveBasedMatchManager.Singleton.Boss.transform);
        #endregion

        #region Private Methods
        #endregion

        #region Event Methods
        private void Surrender() => OnSurrender?.Invoke();
        #endregion
    }
}