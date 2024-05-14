using Code.Player;
using UnityEngine;

namespace Miscellaneous {
    public class TuochDoT : MonoBehaviour {
        #region Public Variables
        [SerializeField, Min(0.1f)] private float m_interval = .15f;
        [SerializeField, Min(0.1f)] private float m_damage = 1;
        #endregion

        #region Private Variables
        private PlayerHealth _health;
        #endregion

        #region Behaviour Callbacks
        private void OnCollisionEnter(Collision other) {
            if (!other.body.CompareTag("Player"))
                return;

            _health = other.gameObject.GetComponent<PlayerHealth>();
            DamageOverTime();
        }

        private void OnCollisionExit(Collision other) {
            if (!other.body.CompareTag("Player"))
                return;

            CancelInvoke();
            _health = null;
        }
        #endregion

        #region Private Methods
        private void DamageOverTime() {
            _health.DealDamage(m_damage, gameObject);
            Invoke(nameof(DamageOverTime), m_interval);
        }
        #endregion
    }
}