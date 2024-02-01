using FMODUnity;
using JetBrains.Annotations;
using UnityEngine;

namespace Code.Graphics {
    [RequireComponent(typeof(Animator))]
    public class BossAnimator : MonoBehaviour {
#if UNITY_EDITOR
        public StudioEventEmitter emitter;
#endif

        #region Public Variables
        [SerializeField] private EventReference m_voiceLineEvent;
        [Space]
        [SerializeField] private AnimationClip m_recomposeAnimationClip;
        [SerializeField] private AnimationClip m_shootLoopAnimationClip;
        [SerializeField] private AnimationClip m_laserBeamStartAnimationClip, m_laserBeamStopAnimationClip;

        public event System.Action OnShoot;
        public event System.Action<bool> OnStartStopVoiceLine;
        #endregion

        #region Private Variables
        private Animator _animator;

        private static readonly int AnimProp_IsTalking = Animator.StringToHash("Is Talking");
        #endregion

        #region Properties
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            _animator = GetComponent<Animator>();
        }
        #endregion

        #region Public Methods
        public float AnimateFireBallsAttack(int rounds) {
            _animator.CrossFade("Boss Shoot (Start)", .25f);
            var length = m_shootLoopAnimationClip.length * rounds;
            Invoke(nameof(StopShooting), length);
            return length;
        }

        public float AnimateLaserBeamAttack(float duration) {
            _animator.CrossFade("Boss Laser Beam (Start)", .25f);
            var length = m_laserBeamStartAnimationClip.length + m_laserBeamStopAnimationClip.length + duration;
            Invoke(nameof(StopLaserBeam), length);
            return length;
        }

        public void AnimateVoiceLine(float duration) {
            Invoke(nameof(StartVoiceLine), m_recomposeAnimationClip.length);
            _animator.CrossFade("Boss Recompose", .25f);
            _animator.SetBool(AnimProp_IsTalking, true);
            Invoke(nameof(StopVoiceLine), duration + m_recomposeAnimationClip.length);
        }

        public void AnimateAttack(int phase, float duration = 0f) {
            switch (phase) {
                case 0:
                    _animator.CrossFade("Shoot", .25f);
                    break;
                case 1:
                    _animator.CrossFade("Boss Laser Beam (Start)", .25f);
                    Invoke(nameof(StopLaserBeam), duration);
                    break;
                case 2:
                    Invoke(nameof(StartVoiceLine), m_recomposeAnimationClip.length);
                    _animator.CrossFade("Boss Recompose", .25f);
                    _animator.SetBool(AnimProp_IsTalking, true);
                    Invoke(nameof(StopVoiceLine), duration + m_recomposeAnimationClip.length);
                    break;
                default:
                    Debug.LogWarning("Phase was not defined...\n", this);
                    return;
            }
        }
        #endregion

        #region Private Methods
        private void StopShooting() => _animator.CrossFade("Boss Shoot (End)", .25f);

        private void StopLaserBeam() => _animator.CrossFade("Boss Laser Beam (End)", .25f);

        private void StartVoiceLine() {
            var evt = RuntimeManager.CreateInstance(m_voiceLineEvent);
            evt.start();
            OnStartStopVoiceLine?.Invoke(true);
        }
        private void StopVoiceLine() {
            _animator.CrossFade("Boss Decompose", .25f);
            _animator.SetBool(AnimProp_IsTalking, false);
            OnStartStopVoiceLine?.Invoke(false);
        }

        [UsedImplicitly]
        private void Shoot() => OnShoot?.Invoke();
        #endregion

#if UNITY_EDITOR
        [ContextMenu("Attack/Phase 1")]
        private void AttackPhaseOne() => AnimateAttack(0);
        [ContextMenu("Attack/Phase 2")]
        private void AttackPhaseTwo() => AnimateAttack(1, 3f);
        [ContextMenu("Attack/Phase 3")]
        private void AttackPhaseThree() {
            emitter.Play();
            RuntimeManager.GetEventDescription(emitter.EventReference).getLength(out var lenMs);
            AnimateAttack(2, 2f);
        }
#endif

        #region Event Methods
        #endregion
    }
}