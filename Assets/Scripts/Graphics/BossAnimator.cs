using FMODUnity;
using JetBrains.Annotations;
using UnityEngine;

namespace Code.Graphics {
    [RequireComponent(typeof(Animator))]
    public class BossAnimator : MonoBehaviour {
        #region Public Variables
#if UNITY_EDITOR
        public StudioEventEmitter emitter;
#endif

        public event System.Action OnShoot;
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
                    _animator.CrossFade("Boss Recompose", .25f);
                    _animator.SetBool(AnimProp_IsTalking, true);
                    Invoke(nameof(StopVoiceLine), duration);
                    break;
                default:
                    Debug.LogWarning("Phase was not defined...\n", this);
                    return;
            }
        }
        #endregion

        #region Private Methods
        private void StopLaserBeam() => _animator.CrossFade("Boss Laser Beam (End)", .25f);
        private void StopVoiceLine() {
            _animator.CrossFade("Boss Decompose", .25f);
            _animator.SetBool(AnimProp_IsTalking, false);
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
            AnimateAttack(2, (lenMs / 1000f) - .15f);
        }
#endif

        #region Event Methods
        #endregion
    }
}