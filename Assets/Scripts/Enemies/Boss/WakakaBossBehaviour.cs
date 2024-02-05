﻿using System.Collections;
using Code.EnemySystem.Boss.Phases;
using Code.EnemySystem.Wakakas;
using Code.Graphics;
using Code.Player;
using Miscellaneous;
using UnityEngine;

namespace Code.EnemySystem.Boss {
    [RequireComponent(typeof(WakakaHealth))]
    public class WakakaBossBehaviour : MonoBehaviour {
        private enum WakakaBossState {
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

        [Header("References")]
        [SerializeField] private BossAnimator m_bossAnimator;
        [SerializeField] private Animator m_animator;

        public event System.Action OnSurrender; 
        #endregion

        #region Private Variables
        private WakakaHealth _health;
        private Transform _target;

        private Coroutine _switchPhaseCoroutine;

        private BossPhaseBase _currentPhase;
        private bool _freezeExecution;

        private static readonly int AnimProp_LookAtWeight = Animator.StringToHash("Look At Weight");
        #endregion

        #region Properties
        public bool IsSwitchingPhase => _switchPhaseCoroutine != null;
        public BossAnimator BossAnimator => m_bossAnimator;
        public Transform Target => _target;
        public Animator Animator => m_animator;

        private WakakaBossState Phase { get; set; } = WakakaBossState.None;
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            _health = GetComponent<WakakaHealth>();

            _health.OnHealthChanged += CheckPhase;
            _health.OnDeath += Die;

            CutsceneIntroController.OnIntroStartStop += HandleCutscene;

            m_phaseOne.SetUp(this);
            m_phaseTwo.SetUp(this);
            m_phaseThree.SetUp(this);
            m_phaseSurrender.SetUp(this);
        }

        private void Start() => _target = PlayerController.Singleton.transform;

        private void Update() {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F11))
                SetPhase(WakakaBossState.PhaseOne);
#endif

            if (_freezeExecution)
                return;

            LookAtPlayer();

            if (Phase is WakakaBossState.None or WakakaBossState.Transitioning)
                return;

            _currentPhase?.Execute();
        }

        private void OnGUI() {
            _currentPhase?.OnGUI();
            GUILayout.Label(Phase.ToString());
        }

        private void OnDestroy() {
            _health.OnHealthChanged -= CheckPhase;
            _health.OnDeath -= Die;

            CutsceneIntroController.OnIntroStartStop -= HandleCutscene;
        }
        #endregion

        #region Public Methods
        public void BeginFight() => SetPhase(WakakaBossState.PhaseOne);

        public void Surrender() {
            m_bossAnimator.AnimateDeath();
            m_animator.enabled = false;
            Destroy(this);
            Destroy(_health);
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
            _health.enabled = false;

            _currentPhase?.End();

            var animTime = m_bossAnimator.AnimateRecompose();
            yield return new WaitForSeconds(animTime);

            animTime = m_bossAnimator.AnimateVoiceLineAuto();
            yield return new WaitForSeconds(animTime);

            animTime = m_bossAnimator.AnimateDecompose();
            yield return new WaitForSeconds(animTime);

            Phase = phase;

            _currentPhase = GetPhase(Phase);
            _currentPhase?.Begin();

            _health.enabled = true;
            _switchPhaseCoroutine = null;
        }

        private BossPhaseBase GetPhase(WakakaBossState phase) => phase switch {
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
            _health.enabled = !_freezeExecution;
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