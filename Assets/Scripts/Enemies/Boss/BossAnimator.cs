using Audio;
using Code.Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Code.EnemySystem.Boss {
    [RequireComponent(typeof(Animator))]
    public class BossAnimator : MonoBehaviour {
        #region Public Variables
        [Header("References")]
        [SerializeField] private GameObject m_shieldObject;

        [Header("Sounds")]
        [SerializeField] private SoundSO m_deathSoundEvent;

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

        private static readonly int AnimProp_IsTalking = Animator.StringToHash("Is Talking");
        #endregion

        #region Properties
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            _animator = GetComponent<Animator>();
            AnimateShieldOnOff(true);
        }

        private void Update() {
            if (GameEvents.IsOnHold)
                _animator.speed = 0f;
            else
                _animator.speed = 1f;
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

        public float AnimateVoiceLineAuto(Sound voiceLine) {
            StartVoiceLine(voiceLine);
            _animator.SetBool(AnimProp_IsTalking, true);
            var duration = voiceLine.Clip.length; // TODO - Replace with audio clip length
            Invoke(nameof(StopVoiceLine), duration);
            return duration;
        }

        public void CancelVoiceLine() => StopVoiceLine();

        public float AnimateRecompose() {
            _animator.CrossFade("Boss Recompose", .25f);
            return m_recomposeAnimationClip.length;
        }

        public float AnimateDecompose() {
            _animator.CrossFade("Boss Decompose", .25f);
            return m_decomposeAnimationClip.length;
        }

        public void AnimateShieldOnOff(bool off) => m_shieldObject.SetActive(!off);

        public void AnimateDeath() => AudioManager.Singleton.PlayOneShotWorldAttached(m_deathSoundEvent.GetSound(), gameObject, MixerType.Voice);
        #endregion

        #region Private Methods
        private void StopShooting() => _animator.CrossFade("Boss Shoot (End)", .25f);

        private void StopLaserBeam() => _animator.CrossFade("Boss Laser Beam (End)", .25f);

        private void StartVoiceLine(Sound voiceLine) {
            AudioManager.Singleton.PlayOneShotWorldAttached(voiceLine, gameObject, MixerType.Voice);
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