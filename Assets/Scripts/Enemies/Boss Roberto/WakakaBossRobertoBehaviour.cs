using System.Collections;
using Audio;
using Code.Core;
using Code.EnemySystem.Boss.Phases;
using Code.EnemySystem.Wakakas;
using Enemies.BossRoberto.Phases;
using SteamIntegration.Achievements;
using UnityEngine;

namespace Enemies.BossRoberto {
    [RequireComponent(typeof(WakakaHealth))]
    public class WakakaBossRobertoBehaviour : MonoBehaviour {
        public enum WakakaBossState {
            None,
            PhaseIdle,
            PhaseMove,
            PhaseFight,
            Surrender
        }

        #region Public Variables
        [Header("Settings")]
        [SerializeField] private float m_rotateLerpQuickness = 12f;
        [SerializeField] private bool m_rotationIgnoreY = true;

        [Space]
        [SerializeField] private BossRobertoPhaseIdle m_phaseIdle;
        [SerializeField] private BossRobertoPhaseMove m_phaseMove;
        [SerializeField] private BossRobertoPhaseFight m_phaseFight;
        [SerializeField] private BossRobertoPhaseSurrender m_phaseSurrender;

        [Space]
        [SerializeField] private SoundSO m_phaseChangeVoiceLine;

        [Header("References")]
        [SerializeField] private BossRobertoAnimator m_bossAnimator;
        [SerializeField] private Animator m_animator;

        [Header("References")]
        [SerializeField] private SteamAchievementSO m_deadMemeAchievement;

        public event System.Action OnBeginFight;
        public event System.Action OnSurrender;
        #endregion

        #region Private Variables
        private Transform _target;

        private Coroutine _switchPhaseCoroutine;
        private Coroutine _moveCoroutine;

        private BossPhaseBase<WakakaBossRobertoBehaviour> _currentPhase;
        private bool _freezeExecution = true;

        private static readonly int AnimProp_LookAtWeight = Animator.StringToHash("Look At Weight");
        #endregion

        #region Properties
        public bool IsSwitchingPhase => _switchPhaseCoroutine != null;

        public Transform Target => _target;
        public BossRobertoAnimator BossAnimator => m_bossAnimator;
        public Animator Animator => m_animator;
        public WakakaHealth Health { get; private set; }

        public bool Enabled { get; set; }

        public WakakaBossState Phase { get; set; } = WakakaBossState.None;
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            Health = GetComponent<WakakaHealth>();

            Health.OnHealthChanged += CheckPhase;
            Health.OnDeath += Die;
            Health.OnEnableDisable += m_bossAnimator.AnimateShieldOnOff;

            GameEvents.OnCutsceneStateChanged += HandleCutscene;

            m_phaseIdle.SetUp(this);
            m_phaseMove.SetUp(this);
            m_phaseFight.SetUp(this);
            m_phaseSurrender.SetUp(this);
        }

        private void Start() => Health.enabled = false;

        private void Update() {
            if (_freezeExecution)
                return;

            LookAtTarget();

            if (!Enabled)
                return;

            if (Phase == WakakaBossState.None)
                return;

            _currentPhase?.Execute();
        }

        private void OnDestroy() {
            _currentPhase?.End();

            Health.OnHealthChanged -= CheckPhase;
            Health.OnDeath -= Die;

            GameEvents.OnCutsceneStateChanged -= HandleCutscene;
        }
        #endregion

        #region Public Methods
        public void BeginFight(Transform target) {
            _target = target;
            _freezeExecution = false;

            SetPhase(WakakaBossState.PhaseMove);
            OnBeginFight?.Invoke();
        }

        public void Surrender() {
            SteamAchievementsController.Singleton?.AdvanceAchievement(m_deadMemeAchievement);

            m_bossAnimator.AnimateDeath();
            m_animator.enabled = false;
            Health.enabled = false;
            enabled = false;
            m_bossAnimator.AnimateShieldOnOff(true);
            OnSurrender?.Invoke();
        }
        #endregion

        #region Private Methods
        private void LookAtTarget() {
            var dir = _target.position - transform.position;
            if (m_rotationIgnoreY)
                dir.y = 0;
            dir.Normalize();

            var lookAtWeight = Animator.GetFloat(AnimProp_LookAtWeight);

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * m_rotateLerpQuickness * lookAtWeight);
        }

        private BossPhaseBase<WakakaBossRobertoBehaviour> GetPhase(WakakaBossState phase) => phase switch {
            WakakaBossState.PhaseIdle => m_phaseIdle,
            WakakaBossState.PhaseMove => m_phaseMove,
            WakakaBossState.PhaseFight => m_phaseFight,
            WakakaBossState.Surrender => m_phaseSurrender,
            _ => null
        };

        public void SetPhase(WakakaBossState phase) {
            if (Phase == phase)
                return;

            Phase = phase;
            _currentPhase?.End();
            _currentPhase = GetPhase(Phase);
            _currentPhase?.Begin();
        }

        private IEnumerator SwitchPhaseCO(WakakaBossState phase) {
            Phase = WakakaBossState.None;
            Health.enabled = false;

            _currentPhase?.End();

            var animTime = m_bossAnimator.AnimateRecompose();
            yield return new WaitForSeconds(animTime);

            animTime = m_bossAnimator.AnimateVoiceLineAuto(m_phaseChangeVoiceLine.GetSound());
            yield return new WaitForSeconds(animTime);

            animTime = m_bossAnimator.AnimateDecompose();
            yield return new WaitForSeconds(animTime);

            Phase = phase;

            _currentPhase = GetPhase(Phase);
            _currentPhase?.Begin();

            Health.enabled = true;
            _switchPhaseCoroutine = null;
        }
        #endregion

        #region Event Methods
        private void HandleCutscene(bool isPlaying) {
            _freezeExecution = isPlaying;
            Health.enabled = !_freezeExecution && Phase != WakakaBossState.None;
        }

        private void CheckPhase(float health) {
            switch (Phase) {
                case WakakaBossState.Surrender:
                    break;
                case WakakaBossState.PhaseIdle:
                    break;
                case WakakaBossState.PhaseMove:
                    break;
                case WakakaBossState.PhaseFight:
                    break;
                case WakakaBossState.None:
                default:
                    return;
            }
        }
        private void Die() => SetPhase(WakakaBossState.Surrender);
        #endregion
    }
}