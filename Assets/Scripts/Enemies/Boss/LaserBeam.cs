using Code.Core;
using Code.Player;
using UnityEngine;

namespace Code.EnemySystem.Boss {
    [RequireComponent(typeof(CapsuleCollider))]
    public class LaserBeam : MonoBehaviour {
        #region Public Variables
        [Header("Settings")]
        [SerializeField, Min(0.1f)] private float m_range = 5f;
        [SerializeField] private float m_trackWeight = .5f;
        [SerializeField] private AnimationCurve m_trackingPrecision = AnimationCurve.EaseInOut(0f, 1f, .5f, .5f);
        [SerializeField, Min(0.01f)] private float m_damageOverTime = 2f;
        [SerializeField, Min(0.02f)] private float m_damageRate = .5f;

        [Header("References")]
        [SerializeField] private Transform m_hitParticle;
        [SerializeField] private LineRenderer m_lineRenderer;
        #endregion

        #region Private Variables
        private Transform _target;
        private CapsuleCollider _collider;

        private PlayerHealth _playerHealth;
        #endregion

        #region Behaviour Callbacks
        private void Awake() => _collider = GetComponent<CapsuleCollider>();

        private void Update() {
            var didHit = Physics.Raycast(transform.position, transform.forward, out var hit, m_range);

            _collider.height = didHit ? hit.distance : m_range;
            _collider.center = _collider.height * .5f * Vector3.forward;
            m_lineRenderer.SetPosition(1, Vector3.forward * _collider.height);
            m_hitParticle.gameObject.SetActive(true);

            if (didHit)
                m_hitParticle.position = hit.point;
            else
                m_hitParticle.position = transform.position + transform.forward * _collider.height;
        }

        private void FixedUpdate() {
            if (!_target || GameEvents.IsOnHold)
                return;

            var t = transform;
            var currentRotation = t.rotation;

            var targetDirection = _target.position - t.position;
            targetDirection.Normalize();

            var desiredRotation = Quaternion.LookRotation(targetDirection);

            transform.rotation = Quaternion.Slerp(currentRotation, desiredRotation, Time.deltaTime * m_trackWeight * m_trackingPrecision.Evaluate(Time.time));
        }

        private void OnTriggerEnter(Collider other) {
            if (!other.TryGetComponent(out _playerHealth))
                return;

            _playerHealth.GetDamage(m_damageOverTime);
            InvokeRepeating(nameof(ApplyDot), m_damageRate, m_damageRate);
        }

        private void OnTriggerExit(Collider other) {
            if (!other.TryGetComponent(out _playerHealth))
                return;

            CancelInvoke(nameof(ApplyDot));
            _playerHealth = null;
        }
        #endregion

        #region Public Methods
        public void AnchorTo(Transform point) => transform.SetParent(point, true);
        public void SetTime(float duration) => Destroy(gameObject, duration);

        public void Track(Transform target) => _target = target;
        #endregion

        #region Private Methods
        private void ApplyDot() {
            if (GameEvents.IsOnHold || !_playerHealth)
                return;

            _playerHealth.GetDamage(m_damageOverTime);
        }
        #endregion
    }
}