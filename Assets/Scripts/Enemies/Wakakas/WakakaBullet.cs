using Code.Player;
using UnityEngine;

namespace Code.EnemySystem.Wakakas {
    [RequireComponent(typeof(Rigidbody))]
    public class WakakaBullet : MonoBehaviour {
        #region Public Variables
        [SerializeField] private float m_damage = 1;
        [SerializeField] private float m_lifeTime = 5;
        #endregion

        #region Private Variables
        private Rigidbody _body;
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            _body = GetComponent<Rigidbody>();
            Destroy(gameObject, m_lifeTime);
        }

        private void OnCollisionEnter(Collision other) {
            if (other.gameObject.TryGetComponent(out PlayerHealth health))
                health.GetDamage(m_damage);
            Destroy(gameObject);
        }
        #endregion

        #region Public Methods
        public void OverrideDamage(float damage) => m_damage = damage;

        public void Launch(Vector3 velocity) {
            _body.velocity = velocity;
            transform.forward = velocity;
        }
        #endregion
    }
}