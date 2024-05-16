using Code.Core.MatchManagers;
using Code.EnemySystem.Boss;
using Code.Promises;
using SteamIntegration.Achievements;
using UnityEngine;
using Utilities;

namespace Code.GameModeUtils.WaveBasedMode {
    [RequireComponent(typeof(WakakaBossBehaviour))]
    public class WaveBasedBossEntity : MonoBehaviour, IEntity {
        #region Public Variables
        [Header("Achievement")]
        [SerializeField] private SteamAchievementSO m_trueEndingAchievement;

        public event System.Action OnSurrender;
        public event System.Action<bool> OnTriggered;
        #endregion

        #region Private Variables
        private WakakaBossBehaviour _controller;
        #endregion

        #region Properties
        public bool IsFighting => _controller.Enabled;
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            _controller = GetComponent<WakakaBossBehaviour>();
            _controller.OnSurrender += Surrender;

            Disable();
        }

        private void Start() => WaveBasedMatchManager.Singleton.SetBoss(this);

        private void Update() {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F1) && _controller.Phase == WakakaBossBehaviour.WakakaBossState.None) {
                Enable();
                StartFight();
            }
            if (Input.GetKeyDown(KeyCode.F2) && _controller.Phase != WakakaBossBehaviour.WakakaBossState.None) {
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
        public void StartFight() {
            _controller.BeginFight();
            OnTriggered?.Invoke(true);
        }
        #endregion

        #region Event Methods
        private void Surrender() {
            if (WaveBasedMatchManager.Singleton.EntityManager.Entities.None())
                SteamAchievementsController.Singleton?.AdvanceAchievement(m_trueEndingAchievement);

            OnSurrender?.Invoke();
            OnTriggered?.Invoke(false);
        }
        #endregion
    }
}