using Audio;
using Code.Core;
using UnityEngine;

namespace Enemies.BossRoberto {
    public class BossRobertoAnimator : MonoBehaviour {
        #region Public Variables
        [Header("References")]
        [SerializeField] private GameObject m_shieldObject;

        [Header("Sounds")]
        [SerializeField] private SoundSO m_deathSoundEvent;

        [Header("Animations")]
        [SerializeField] private AnimationClip m_recomposeAnimationClip;
        [SerializeField] private AnimationClip m_decomposeAnimationClip;

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
        public float AnimateCamerasAttack(float time) => time;

        public float AnimateVoiceLineAuto(Sound voiceLine) {
            StartVoiceLine(voiceLine);
            // _animator.SetBool(AnimProp_IsTalking, true);
            var duration = voiceLine.Clip.length;
            Invoke(nameof(StopVoiceLine), duration);
            return duration;
        }

        public void CancelVoiceLine() => StopVoiceLine();

        public float AnimateRecompose() {
            _animator.CrossFade("Recompose", .25f);
            return m_recomposeAnimationClip.length;
        }

        public float AnimateDecompose() {
            _animator.CrossFade("Decompose", .25f);
            return m_decomposeAnimationClip.length;
        }

        public void AnimateShieldOnOff(bool off) => m_shieldObject.SetActive(!off);

        public void AnimateDeath() => AudioManager.Singleton.PlayOneShotWorldAttached(m_deathSoundEvent.GetSound(), gameObject, MixerType.Voice);
        #endregion

        #region Private Methods
        private void StartVoiceLine(Sound voiceLine) {
            AudioManager.Singleton.PlayOneShotWorldAttached(voiceLine, gameObject, MixerType.Voice);
            OnStartStopVoiceLine?.Invoke(true);
        }

        private void StopVoiceLine() {
            // _animator.SetBool(AnimProp_IsTalking, false);
            OnStartStopVoiceLine?.Invoke(false);
        }
        #endregion

        #region Event Methods
        #endregion
    }
}