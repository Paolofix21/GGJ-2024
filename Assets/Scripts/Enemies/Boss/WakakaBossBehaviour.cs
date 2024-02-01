﻿using System;
using System.Collections;
using Code.EnemySystem.Boss.Phases;
using Code.Graphics;
using Code.Player;
using UnityEngine;

namespace Code.EnemySystem.Boss {
    [RequireComponent(typeof(WakakaHealth))]
    [RequireComponent(typeof(BossAttackFireBalls))]
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

        [Header("References")]
        [SerializeField] private BossAnimator m_bossAnimator;
        [SerializeField] private Animator m_animator;
        #endregion

        #region Private Variables
        private WakakaHealth _health;
        private BossAttackFireBalls _attackFireBalls;
        private BossAttackLaserBeams _attackLaserBeams;

        private Transform _target;

        private Coroutine _switchPhaseCoroutine;

        private BossPhaseBase _currentPhase;
        private static readonly int AnimProp_LookAtWeight = Animator.StringToHash("Look At Weight");
        #endregion

        #region Properties
        public bool IsSwitchingPhase => _switchPhaseCoroutine != null;
        public BossAnimator BossAnimator => m_bossAnimator;
        public Animator Animator => m_animator;

        private WakakaBossState Phase { get; set; } = WakakaBossState.None;
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            _health = GetComponent<WakakaHealth>();
            _attackFireBalls = GetComponent<BossAttackFireBalls>();
            _attackLaserBeams = GetComponent<BossAttackLaserBeams>();

            _health.OnHealthChanged += CheckPhase;
            _health.OnDeath += Die;

            m_bossAnimator.OnStartStopVoiceLine += OnVoiceLineStartStop;
            m_bossAnimator.OnShoot += Shoot;

            m_phaseOne.SetUp(this);
            m_phaseTwo.SetUp(this);
            // m_phaseThree.SetUp(this);
        }

        private void Start() => _target = PlayerController.Singleton.transform;

        private void Update() {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F11))
                SetPhase(WakakaBossState.PhaseOne);
#endif

            LookAtPlayer();

            if (Phase is WakakaBossState.None or WakakaBossState.Transitioning)
                return;

            _currentPhase?.Execute();
        }

        private void OnDestroy() {
            _health.OnHealthChanged -= CheckPhase;
            _health.OnDeath -= Die;

            m_bossAnimator.OnStartStopVoiceLine -= OnVoiceLineStartStop;
            m_bossAnimator.OnShoot -= Shoot;
        }
        #endregion

        #region Public Methods
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
            const float duration = 3f;

            Phase = WakakaBossState.Transitioning;

            _currentPhase?.End();

            // m_bossAnimator.AnimateAttack(2, duration);
            m_bossAnimator.AnimateVoiceLine(2f);

            yield return new WaitForSeconds(duration);

            Phase = phase;

            _currentPhase = GetPhase(Phase);
            _currentPhase?.Begin();

            _switchPhaseCoroutine = null;
        }

        private BossPhaseBase GetPhase(WakakaBossState phase) => phase switch {
            WakakaBossState.PhaseOne => m_phaseOne,
            WakakaBossState.PhaseTwo => m_phaseTwo,
            // WakakaBossState.PhaseThree => m_phaseThree,
            _ => null
        };
        #endregion

        #region Event Methods
        private void Shoot() {
            switch (Phase) {
                case WakakaBossState.PhaseOne:
                    _attackFireBalls.ShootAt(_target);
                    break;
                case WakakaBossState.PhaseTwo:
                    _attackLaserBeams.ShootAt(m_phaseTwo.Duration, _target);
                    break;
                case WakakaBossState.PhaseThree:
                    break;
                case WakakaBossState.Surrender:
                    break;
                case WakakaBossState.Transitioning:
                case WakakaBossState.None:
                default:
                    return;
            }
        }

        private void OnVoiceLineStartStop(bool started) => _health.enabled = !started;

        private void CheckPhase(float health) {
            switch (Phase) {
                case WakakaBossState.PhaseOne:
                    if (health <= .66f)
                        SetPhase(WakakaBossState.PhaseTwo);
                    break;
                case WakakaBossState.PhaseTwo:
                    if (health <= .33f)
                        SetPhase(WakakaBossState.PhaseTwo);
                    break;
                case WakakaBossState.PhaseThree:
                    break;
                case WakakaBossState.Surrender:
                case WakakaBossState.None:
                default:
                    return;
            }
        }
        private void Die() {
            SetPhase(WakakaBossState.Surrender);
        }

        private void SetPhase(WakakaBossState phase) {
            if (Phase == phase)
                return;

            if (_switchPhaseCoroutine != null)
                StopCoroutine(_switchPhaseCoroutine);

            _switchPhaseCoroutine = StartCoroutine(SwitchPhaseCO(phase));
        }
        #endregion
    }
}