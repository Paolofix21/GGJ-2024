using System.Collections;
using Audio;
using Code.Promises;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.GameModeUtils.WaveBasedMode {
    public class SpawnPoint : MonoBehaviour, IPointOfInterest {
        #region Public Variables
        [Header("References")]
        [SerializeField] private Transform m_portal;
        [field: SerializeField] public bool Omit { get; private set; }

        [Header("Settings")]
        [SerializeField] private float m_animationDuration = 1f;
        [SerializeField] private float m_defaultScale = 0f;
        [FormerlySerializedAs("m_targetScale")] [SerializeField] private float m_desiredScale = 1f;

        [Space]
        [SerializeField] private SoundSO m_appearSound;
        [SerializeField] private SoundSO m_vanishSound;
        #endregion

        #region Private Variables
        private Coroutine _animationCoroutine;
        #endregion

        #region IPointOfInterest
        public Vector3 Position => transform.position;
        public Quaternion Rotation => transform.rotation;

        public bool IsVisible { get; private set; }
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            m_portal.localScale = Vector3.one * m_defaultScale;
            m_portal.gameObject.SetActive(m_defaultScale > 0);
        }
        #endregion

        #region Public Methods
        public void AnimatePortal(bool appear) {
            if (this == null)
                return;

            if (_animationCoroutine != null)
                StopCoroutine(_animationCoroutine);
            _animationCoroutine = StartCoroutine(AnimatePortalCO(appear));
        }
        #endregion

        #region Private Methods
        private IEnumerator AnimatePortalCO(bool appear = true) {
            if (appear)
                yield return new WaitForSeconds(Random.value);

            var defaultScale = Vector3.one * m_defaultScale;
            var desiredScale = Vector3.one * m_desiredScale;

            var targetScale = appear ? desiredScale : defaultScale;
            var sourceScale = appear ? defaultScale : targetScale;

            m_portal.localScale = sourceScale;
            if (appear) {
                IsVisible = true;
                m_portal.gameObject.SetActive(true);
            }

            AudioManager.Singleton.PlayOneShotWorldAttached((appear ? m_appearSound : m_vanishSound).GetSound(), gameObject, MixerType.SoundFx);

            var t = 0f;

            while (t < m_animationDuration) {
                t += Time.deltaTime;
                m_portal.localScale = Vector3.Slerp(sourceScale, targetScale, t / m_animationDuration);
                yield return null;
            }

            m_portal.localScale = targetScale;
            if (!appear) {
                m_portal.gameObject.SetActive(m_defaultScale > 0);
                IsVisible = false;
            }

            _animationCoroutine = null;
        }
        #endregion
    }
}