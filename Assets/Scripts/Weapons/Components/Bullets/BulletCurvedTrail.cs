using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

namespace Weapons.Components {
    public class BulletCurvedTrail : MonoBehaviour {
        #region Public Variables
        [SerializeField, Min(0.01f)] private float m_time = .25f;
        [SerializeField] private Vector3 m_startDirection = new(0, 0, 1);
        #endregion

        #region Private Variables
        private TrailRenderer _trailRenderer;

        private BezierCurve _curve;
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
            var direction = point - transform.position;
            _curve = BezierCurve.FromTangent(transform.position, transform.TransformDirection(m_startDirection), point, direction.normalized);
            StartCoroutine(AnimateCO());
        }
        #endregion

        #region Private Methods
        private IEnumerator AnimateCO() {
            var t = 0f;
            while (t < m_time) {
                t += Time.deltaTime;
                transform.position = CurveUtility.EvaluatePosition(_curve, t / m_time);
                yield return null;
            }

            Destroy(gameObject, _trailRenderer.time);
        }
        #endregion
    }
}