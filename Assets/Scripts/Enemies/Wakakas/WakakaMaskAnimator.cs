﻿using System.Collections;
using FMOD;
using FMODUnity;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Code.EnemySystem.Wakakas {
    [DefaultExecutionOrder(-1)]
    public class WakakaMaskAnimator : MonoBehaviour {
        #region Public Variables
        [Header("Intro")]
        [SerializeField] private EventReference m_introVoiceLineEvent;

        [Header("Laughter")]
        [SerializeField] private AnimationCurve m_laughterAnimation;
        [SerializeField] private EventReference m_laughterClipEvent;
        [SerializeField] private int m_laughterShapeIndex;

        [Header("Death")]
        [SerializeField] private AnimationCurve m_deathScaleAnimation;
        [SerializeField] private EventReference m_deathSound;
        [SerializeField] private ParticleSystem m_deathParticle;
        [SerializeField] private float m_deathRotationSpeed = 720f;
        #endregion

        #region Private Variables
        private SkinnedMeshRenderer _meshRenderer;
        private TrailRenderer _trailRenderer;

        private MaterialPropertyBlock _block;
        private MaterialPropertyBlock _trailBlock;

        private Coroutine _deathCoroutine;

        private bool _animatingLaughter;
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            _meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        }
        #endregion

        #region Public Methods
        [ContextMenu("Laugh")]
        public void AnimateLaughter() {
            if (!isActiveAndEnabled)
                return;

            if (_animatingLaughter)
                return;

            StartCoroutine(LaughCO());
            Debug.Log("Laughing\n");
        }

        [ContextMenu("Laugh")]
        public void AnimateIntroVoiceLine() => RuntimeManager.PlayOneShotAttached(m_introVoiceLineEvent, gameObject);

        [ContextMenu("Laugh")]
        public void AnimateDeath() {
            if (_deathCoroutine != null)
                return;

            _deathCoroutine = StartCoroutine(DeathCO());

            RuntimeManager.PlayOneShotAttached(m_deathSound, gameObject);
        }
        #endregion

        #region Private Methods
        private IEnumerator LaughCO() {
            _animatingLaughter = true;

            var t = 0f;
            var duration = m_laughterAnimation.keys[^1].time;

            if (!m_laughterClipEvent.IsNull) {
                RuntimeManager.PlayOneShotAttached(m_laughterClipEvent, gameObject);
            }

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
            Destroy(gameObject);
        }
        #endregion
    }
}