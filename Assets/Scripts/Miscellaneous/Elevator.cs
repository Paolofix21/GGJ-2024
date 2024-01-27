using UnityEditor;
using UnityEngine;

namespace Miscellaneous {
    public class Elevator : MonoBehaviour {
        #region Public Variables
        [SerializeField] private float m_height = 5f;
        [SerializeField] private float m_speed = .5f;
        [SerializeField] private Transform m_platform;
        #endregion

        #region Private Variables
        private Vector3 _originalPos, _targetPos;

        private bool _elevating;
        private float _pos;
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            _originalPos = m_platform.position;
            _targetPos = _originalPos + Vector3.up * m_height;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            if (EditorApplication.isPlaying)
                return;

            if (!m_platform)
                return;

            Gizmos.DrawSphere(m_platform.position + Vector3.up * m_height, .5f);
            Gizmos.DrawLine(m_platform.position, m_platform.position + Vector3.up * m_height);
        }
#endif

        private void FixedUpdate() {
            _pos = Mathf.Clamp01(_pos + m_speed * (_elevating ? Time.fixedDeltaTime : -Time.fixedDeltaTime));
            m_platform.position = Vector3.Lerp(_originalPos, _targetPos, _pos);
        }

        private void OnTriggerEnter(Collider other) {
            if (!other.CompareTag("Player"))
                return;

            _elevating = true;
        }

        private void OnTriggerExit(Collider other) {
            if (!other.CompareTag("Player"))
                return;

            _elevating = false;
        }
        #endregion
    }
}