using FMOD;
using FMOD.Studio;
using FMODUnity;
using JetBrains.Annotations;
using UnityEngine;
using Debug = UnityEngine.Debug;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Code.EnemySystem.Boss {
    [RequireComponent(typeof(Animator))]
    public class BossAnimator : MonoBehaviour {
        #region Public Variables
        [Header("Sounds")]
        [SerializeField] private EventReference m_voiceLineEvent;
        [SerializeField] private EventReference m_deathSoundEvent;

        [Header("Animations")]
        [SerializeField] private AnimationClip m_recomposeAnimationClip;
        [SerializeField] private AnimationClip m_decomposeAnimationClip;
        [SerializeField] private AnimationClip m_shootLoopAnimationClip;
        [SerializeField] private AnimationClip m_laserBeamStartAnimationClip, m_laserBeamStopAnimationClip;
        [SerializeField] private AnimationClip m_trapezioAttackAnimationClip;

        public event System.Action OnShoot;
        public event System.Action<bool> OnStartStopVoiceLine;
        #endregion

        #region Private Variables
        private Animator _animator;

        private EventInstance _voiceLinePlayer, _deathSoundPlayer;

        private static readonly int AnimProp_IsTalking = Animator.StringToHash("Is Talking");
        #endregion

        #region Properties
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            _animator = GetComponent<Animator>();

            _voiceLinePlayer = RuntimeManager.CreateInstance(m_voiceLineEvent);
            _deathSoundPlayer = RuntimeManager.CreateInstance(m_deathSoundEvent);
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

        public float AnimateTrapezioAttack() {
            _animator.CrossFade("Boss Trapezio Attack", .25f);
            return m_trapezioAttackAnimationClip.length;
        }

        public float AnimateVoiceLine(float duration) {
            Invoke(nameof(StartVoiceLine), m_recomposeAnimationClip.length);
            _animator.CrossFade("Boss Recompose", .25f);
            _animator.SetBool(AnimProp_IsTalking, true);
            Invoke(nameof(StopVoiceLine), duration + m_recomposeAnimationClip.length);
            return duration + m_recomposeAnimationClip.length;
        }

        public float AnimateVoiceLineAuto() {
            _voiceLinePlayer.set3DAttributes(transform.To3DAttributes());
            _voiceLinePlayer.setVolume(5f);
            _voiceLinePlayer.start();
            _voiceLinePlayer.getDescription(out var description);
            description.getLength(out var lenMs);

            _animator.SetBool(AnimProp_IsTalking, true);
            var duration = Mathf.Max(1f, lenMs / 1000f);
            Invoke(nameof(StopVoiceLine), duration);
            return duration;
        }

        public float AnimateRecompose() {
            _animator.CrossFade("Boss Recompose", .25f);
            return m_recomposeAnimationClip.length;
        }

        public float AnimateDecompose() {
            _animator.CrossFade("Boss Decompose", .25f);
            return m_decomposeAnimationClip.length;
        }

        public void AnimateDeath() {
            _voiceLinePlayer.stop(STOP_MODE.IMMEDIATE);
            _deathSoundPlayer.set3DAttributes(transform.To3DAttributes());
            _deathSoundPlayer.setVolume(5f);
            _deathSoundPlayer.start();
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
            _voiceLinePlayer.start();
            OnStartStopVoiceLine?.Invoke(true);
        }
        private void StopVoiceLine() {
            _animator.SetBool(AnimProp_IsTalking, false);
            OnStartStopVoiceLine?.Invoke(false);
        }

        [UsedImplicitly]
        private void Shoot() => OnShoot?.Invoke();
        #endregion
    }
}