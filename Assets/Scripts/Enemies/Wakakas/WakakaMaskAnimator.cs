using System.Collections;
using Audio;
using UnityEngine;

namespace Code.EnemySystem.Wakakas {
    [DefaultExecutionOrder(-1)]
    public class WakakaMaskAnimator : MonoBehaviour {
        #region Public Variables
        [Header("Intro")]
        [SerializeField] private SoundSO m_introVoiceLineEvent;

        [Header("Laughter")]
        [SerializeField] private AnimationCurve m_laughterAnimation;
        [SerializeField] private SoundSO m_laughterClipEvent;
        [SerializeField] private int m_laughterShapeIndex;

        [Header("Damage")]
        [SerializeField] private float m_damageAnimationDuration = 0.5f;
        [SerializeField] private AnimationCurve m_damageAnimation = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private SoundSO m_hitSound;

        [Header("Death")]
        [SerializeField] private AnimationCurve m_deathScaleAnimation;
        [SerializeField] private SoundSO m_deathPushSound;
        [SerializeField] private SoundSO m_deathExplosionSound;
        [SerializeField] private ParticleSystem m_deathParticle;
        [SerializeField] private float m_deathRotationSpeed = 720f;
        #endregion

        #region Private Variables
        private SkinnedMeshRenderer _meshRenderer;
        private TrailRenderer _trailRenderer;

        private MaterialPropertyBlock _block;
        private MaterialPropertyBlock _trailBlock;

        private Coroutine _damageCoroutine;
        private Coroutine _deathCoroutine;
        private Coroutine _laughterCoroutine;

        private bool _animatingLaughter;

        private static readonly int MatProp_Saturation = Shader.PropertyToID("_Saturation");
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            _meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            _block = new MaterialPropertyBlock();
            _meshRenderer.GetPropertyBlock(_block);
        }
        #endregion

        #region Public Methods
        [ContextMenu("Laugh")]
        public void AnimateLaughter() {
            if (!isActiveAndEnabled)
                return;

            if (_animatingLaughter)
                return;

            if (_laughterCoroutine != null)
                StopCoroutine(_laughterCoroutine);

            _laughterCoroutine = StartCoroutine(LaughCO());
        }

        public void AnimateIntroVoiceLine() => AudioManager.Singleton.PlayOneShotWorldAttached(m_introVoiceLineEvent.GetSound(), gameObject, MixerType.Voice);

        public void AnimateDamage() {
            if (!isActiveAndEnabled)
                return;

            AudioManager.Singleton.PlayOneShot2D(m_hitSound.GetSound(), MixerType.SoundFx);

            if (_damageCoroutine != null)
                return;

            _damageCoroutine = StartCoroutine(DamageCO());
        }

        public void AnimateDeath() {
            if (_deathCoroutine != null)
                return;

            if (_laughterCoroutine != null)
                StopCoroutine(_laughterCoroutine);

            _deathCoroutine = StartCoroutine(DeathCO());

            AudioManager.Singleton.PlayOneShotWorldAttached(m_deathPushSound.GetSound(), gameObject, MixerType.Voice);
        }
        #endregion

        #region Private Methods
        private IEnumerator DamageCO() {
            var t = 0f;

            while (t < m_damageAnimationDuration) {
                t += Time.deltaTime;

                _meshRenderer.GetPropertyBlock(_block);
                _block.SetFloat(MatProp_Saturation, m_damageAnimation.Evaluate(t / m_damageAnimationDuration));
                _meshRenderer.SetPropertyBlock(_block);

                yield return null;
            }

            _meshRenderer.GetPropertyBlock(_block);
            _block.SetFloat(MatProp_Saturation, 1);
            _meshRenderer.SetPropertyBlock(_block);

            _damageCoroutine = null;
        }

        private IEnumerator LaughCO() {
            _animatingLaughter = true;

            var t = 0f;
            var duration = m_laughterAnimation.keys[^1].time;

            AudioManager.Singleton.PlayOneShotWorldAttached(m_laughterClipEvent.GetSound(), gameObject, MixerType.SoundFx);

            while (t < duration) {
                t += Time.deltaTime;
                _meshRenderer.SetBlendShapeWeight(m_laughterShapeIndex, m_laughterAnimation.Evaluate(t) * 100);
                yield return null;
            }

            _meshRenderer.SetBlendShapeWeight(m_laughterShapeIndex, 0f);
            _animatingLaughter = false;
        }

        private IEnumerator DeathCO() {
            var t = 0f;
            var duration = m_deathScaleAnimation.keys[^1].time;

            while (t < duration) {
                t += Time.deltaTime;
                transform.localScale = Vector3.one * m_deathScaleAnimation.Evaluate(t);
                transform.Rotate(m_deathRotationSpeed * Time.deltaTime * Vector3.up, Space.World);
                yield return null;
            }

            _deathCoroutine = null;
            Instantiate(m_deathParticle, transform.position, Quaternion.identity);
            AudioManager.Singleton.PlayOneShotWorld(m_deathExplosionSound.GetSound(), transform.position, MixerType.SoundFx);
            Destroy(gameObject);
        }
        #endregion
    }
}