using System.Collections;
using UnityEngine;

namespace Weapons.Components {
    public class BulletTrail : MonoBehaviour {
        #region Public Variables
        [SerializeField, Min(0.01f)] private float m_time = .25f;
        #endregion

        #region Private Variables
        private TrailRenderer _trailRenderer;
        #endregion

        #region Behaviour Callbacks
        private void Awake() => _trailRenderer = GetComponent<TrailRenderer>();
        #endregion

        #region Public Methods
#if UNITY_EDITOR
        [ContextMenu("Shoot")]
        private void Shoot() => SetDestination(transform.position + transform.forward * 10);
#endif

        public void SetDestination(Vector3 point) {
            transform.forward = point - transform.position;
            StartCoroutine(AnimateCO(point));
        }
        #endregion

        #region Private Methods
        private IEnumerator AnimateCO(Vector3 targetPoint) {
            var start = transform.position;

            var t = 0f;
            while (t < m_time) {
                t += Time.deltaTime;
                transform.position = Vector3.Lerp(start, targetPoint, t / m_time);
                yield return null;
            }

            Destroy(gameObject, _trailRenderer.time);
        }
        #endregion
    }
}