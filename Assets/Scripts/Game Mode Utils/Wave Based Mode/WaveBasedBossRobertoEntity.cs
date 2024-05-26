using System.Collections.Generic;
using Code.Core.MatchManagers;
using Code.EnemySystem.Wakakas;
using Code.Promises;
using Enemies.BossRoberto;
using UnityEngine;
using Utilities;

namespace Code.GameModeUtils.WaveBasedMode {
    [RequireComponent(typeof(WakakaBossRobertoBehaviour))]
    public class WaveBasedBossRobertoEntity : MonoBehaviour, IEntity {
        #region Public Variables
        [Header("References")]
        [SerializeField] private List<WakakaDestroyOnDeath> m_cameras = new();

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

        private void Start() {
            m_cameras.ForEach(c => c.OnTerminate += RefreshCameras);
            gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        private void Update() {
            if (Input.GetKeyDown(KeyCode.F3) && _controller.Phase == WakakaBossRobertoBehaviour.WakakaBossState.None) {
                Enable();
                StartFight();
            }
            if (Input.GetKeyDown(KeyCode.F4) && _controller.Phase != WakakaBossRobertoBehaviour.WakakaBossState.None) {
                _controller.Health.ApplyDamage(Mathf.Infinity, _controller.gameObject);
            }
        }
#endif

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

        private void RefreshCameras(WakakaDestroyOnDeath caller) {
            m_cameras.Remove(caller);

            if (m_cameras.Count > 0)
                return;

            if (!WaveBasedMatchManager.Singleton)
                return;

            if (WaveBasedMatchManager.Singleton.Boss.IsFighting)
                OnWavesOver();
            else
                WaveBasedMatchManager.Singleton.OnWavesEnded += OnWavesOver;
        }

        private void OnWavesOver() {
            gameObject.SetActive(true);
            Enable();
            StartFight();
            WaveBasedMatchManager.Singleton.OnWavesEnded -= OnWavesOver;
        }
        #endregion
    }
}