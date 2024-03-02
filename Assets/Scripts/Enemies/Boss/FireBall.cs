using Code.Core;
using Code.EnemySystem.Wakakas;
using Code.Player;
using UnityEngine;

namespace Code.EnemySystem.Boss {
    [RequireComponent(typeof(Rigidbody))]
    public class FireBall : MonoBehaviour {
        #region Public Variables
        [SerializeField] private float m_damage;
        [SerializeField] private float m_homingStrength = 0.5f;
        [SerializeField] private float m_lifeTime = 3f;
        #endregion

        #region Private Variables
        private Rigidbody _body;
        private WakakaHealth _health;

        private Transform _target;
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            _body = GetComponent<Rigidbody>();

            if (TryGetComponent(out _health))
                _health.OnDeath += Die;

            Destroy(gameObject, m_lifeTime);
        }

        private void FixedUpdate() {
            if (GameEvents.IsOnHold) {
                _body.velocity = Vector3.zero;
                return;
            }

            if (!_target)
                return;

            var currentVelocity = _body.velocity;
            var desiredVelocity = _target.position - transform.position;
            desiredVelocity.Normalize();
            desiredVelocity *= currentVelocity.magnitude;

            _body.velocity = Vector3.Slerp(currentVelocity, desiredVelocity, Time.deltaTime * m_homingStrength);
        }

        private void OnCollisionEnter(Collision other) {
            Destroy(gameObject);

            if (!other.gameObject.TryGetComponent(out PlayerHealth health))
                return;

            health.GetDamage(m_damage);
        }
        #endregion

        #region Public Methods
        public void Shoot(Vector3 velocity) => _body.velocity = velocity;

        public void SetHomingTarget(Transform target) => _target = target;
        #endregion

        #region Event Methods
        private void Die() => Destroy(gameObject);
        #endregion
    }
}