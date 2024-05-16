using System.Collections;
using Audio;
using Code.Core;
using Code.EnemySystem.Boss.Phases;
using Code.EnemySystem.Wakakas;
using SteamIntegration.Achievements;
using UnityEngine;
using Utilities;

namespace Code.EnemySystem.Boss {
    [RequireComponent(typeof(WakakaHealth))]
    public class WakakaBossBehaviour : MonoBehaviour {
        public enum WakakaBossState {
            Transitioning = -1,
            None,
            PhaseOne,
            PhaseTwo,
            PhaseThree,
            Surrender
        }

        #region Public Variables
        [Header("Settings")]
        [SerializeField] private float m_rotateLerpQuickness = 12f;
        [SerializeField] private bool m_rotationIgnoreY = true;

        [Space]
        [SerializeField] private BossPhaseOne m_phaseOne;
        [SerializeField] private BossPhaseTwo m_phaseTwo;
        [SerializeField] private BossPhaseThree m_phaseThree;
        [SerializeField] private BossPhaseSurrender m_phaseSurrender;

        [Space]
        [SerializeField] private SoundSO m_phaseChangeVoiceLine;

        [Header("References")]
        [SerializeField] private BossAnimator m_bossAnimator;
        [SerializeField] private Animator m_animator;

        [Header("References")]
        [SerializeField] private SteamAchievementSO m_defeatBossAchievement;
        [SerializeField] private SteamAchievementSO m_slapDefeatBossAchievement;

        public event System.Action OnBeginFight;
        public event System.Action OnSurrender;
        #endregion

        #region Private Variables
        private Transform _target;

        private Coroutine _switchPhaseCoroutine;

        private BossPhaseBase<WakakaBossBehaviour> _currentPhase;
        private bool _freezeExecution;

        private static readonly int AnimProp_LookAtWeight = Animator.StringToHash("Look At Weight");
        #endregion

        #region Properties
        public bool IsSwitchingPhase => _switchPhaseCoroutine != null;
        public BossAnimator BossAnimator => m_bossAnimator;
        public Transform Target => _target;
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

            m_phaseOne.SetUp(this);
            m_phaseTwo.SetUp(this);
            m_phaseThree.SetUp(this);
            m_phaseSurrender.SetUp(this);
        }

        private void Start() {
            _target = GameEvents.MatchManager.GetPlayerEntity().Transform;
            Health.enabled = false;
        }

        private void Update() {
            if (_freezeExecution)
                return;

            LookAtPlayer();

            if (!Enabled)
                return;

            if (Phase is WakakaBossState.None or WakakaBossState.Transitioning)
                return;

            _currentPhase?.Execute();
        }

        private void OnGUI() {
            _currentPhase?.OnGUI();
            GUILayout.Label(Phase.ToString());
        }

        private void OnDestroy() {
            _currentPhase?.End();

            Health.OnHealthChanged -= CheckPhase;
            Health.OnDeath -= Die;

            GameEvents.OnCutsceneStateChanged -= HandleCutscene;
        }
        #endregion

        #region Public Methods
        public void BeginFight() {
            SetPhase(WakakaBossState.PhaseOne);
            OnBeginFight?.Invoke();
        }

        public void Surrender() {
            SteamAchievementsController.Singleton?.AdvanceAchievement(m_defeatBossAchievement);

            if (Health.DamageObject == DamageObject.Whip)
                SteamAchievementsController.Singleton?.AdvanceAchievement(m_slapDefeatBossAchievement);

            m_bossAnimator.AnimateDeath();
            m_animator.enabled = false;
            Health.enabled = false;
            enabled = false;
            m_bossAnimator.AnimateShieldOnOff(true);
            OnSurrender?.Invoke();
        }
        #endregion

        #region Private Methods
        private void LookAtPlayer() {
            var dir = _target.position - transform.position;
            if (m_rotationIgnoreY)
                dir.y = 0;
            dir.Normalize();

            var lookAtWeight = Animator.GetFloat(AnimProp_LookAtWeight);

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * m_rotateLerpQuickness * lookAtWeight);
        }

        private IEnumerator SwitchPhaseCO(WakakaBossState phase) {
            Phase = WakakaBossState.Transitioning;
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

        private BossPhaseBase<WakakaBossBehaviour> GetPhase(WakakaBossState phase) => phase switch {
            WakakaBossState.PhaseOne => m_phaseOne,
            WakakaBossState.PhaseTwo => m_phaseTwo,
            WakakaBossState.PhaseThree => m_phaseThree,
            WakakaBossState.Surrender => m_phaseSurrender,
            _ => null
        };
        #endregion

        #region Event Methods
        private void HandleCutscene(bool isPlaying) {
            _freezeExecution = isPlaying;
            Health.enabled = !_freezeExecution && Phase != WakakaBossState.None;
        }

        private void CheckPhase(float health) {
            switch (Phase) {
                case WakakaBossState.PhaseOne:
                    if (health <= .66f)
                        SetPhase(WakakaBossState.PhaseTwo);
                    break;
                case WakakaBossState.PhaseTwo:
                    if (health <= .33f)
                        SetPhase(WakakaBossState.PhaseThree);
                    break;
                case WakakaBossState.PhaseThree:
                case WakakaBossState.Surrender:
                    break;
                case WakakaBossState.None:
                default:
                    return;
            }
        }
        private void Die() => SetPhase(WakakaBossState.Surrender);

        private void SetPhase(WakakaBossState phase) {
            if (Phase == phase)
                return;

            if (_switchPhaseCoroutine != null)
                StopCoroutine(_switchPhaseCoroutine);

            if (phase != WakakaBossState.Surrender) {
                _switchPhaseCoroutine = StartCoroutine(SwitchPhaseCO(phase));
                return;
            }

            Phase = phase;
            _currentPhase?.End();
            _currentPhase = GetPhase(Phase);
            _currentPhase.Begin();
            _currentPhase.End();
        }
        #endregion
    }
}