using Code.Core;
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

        private WakakaBossBehaviour.WakakaBossState _lastPhase = WakakaBossBehaviour.WakakaBossState.None;
        #endregion

        #region Properties
        public bool IsFighting => _controller.Enabled;
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            _controller = GetComponent<WakakaBossBehaviour>();
            _controller.OnPhaseChange += OnPhaseChange;
            _controller.OnSurrender += Surrender;

            Disable();
        }

        private void Start() => WaveBasedMatchManager.Singleton.SetBoss(this);

#if UNITY_EDITOR
        private void Update() {
            if (Input.GetKeyDown(KeyCode.F1) && _controller.Phase == WakakaBossBehaviour.WakakaBossState.None) {
                Enable();
                StartFight();
            }
            if (Input.GetKeyDown(KeyCode.F2) && _controller.Phase != WakakaBossBehaviour.WakakaBossState.None) {
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
        public void StartFight() {
            _controller.BeginFight();
            OnTriggered?.Invoke(true);

            GameEvents.GetMatchManager<WaveBasedMatchManager>().EntityManager.OnEntityDied += OnEntityDied;
        }
        #endregion

        #region Event Methods
        private void OnEntityDied(IEntity entity) {
            if (entity is WaveBasedWakakaEntity wakaka)
                _controller.Health.ApplyDamage(wakaka.BossEnergy, null);
        }

        private void OnPhaseChange(WakakaBossBehaviour.WakakaBossState phase) => _lastPhase = phase;

        private void Surrender() {
            if (_lastPhase == WakakaBossBehaviour.WakakaBossState.PhaseThree && WaveBasedMatchManager.Singleton.EntityManager.Entities.None())
                SteamAchievementsController.Singleton?.AdvanceAchievement(m_trueEndingAchievement);

            OnSurrender?.Invoke();
            OnTriggered?.Invoke(false);

            GameEvents.GetMatchManager<WaveBasedMatchManager>().EntityManager.OnEntityDied -= OnEntityDied;
        }
        #endregion
    }
}