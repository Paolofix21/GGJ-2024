using Code.Player;
using UnityEngine;

namespace Code.EnemySystem.Boss {
    [RequireComponent(typeof(Rigidbody))]
    public class FireBall : MonoBehaviour {
        #region Public Variables
        [SerializeField] private float m_damage;
        #endregion

        #region Private Variables
        private Rigidbody _body;
        #endregion

        #region Behaviour Callbacks
        private void OnCollisionEnter(Collision other) {
            Destroy(gameObject);

            if (!other.gameObject.TryGetComponent(out PlayerHealth health))
                return;

            health.GetDamage(m_damage);
        }
        #endregion

        #region Public Methods
        public void Shoot(Vector3 velocity) => _body.velocity = velocity;
        #endregion
    }
}