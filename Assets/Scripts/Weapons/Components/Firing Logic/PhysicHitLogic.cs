using UnityEngine;

namespace Code.Weapons {
    [System.Serializable]
    public class PhysicHitLogic : FiringLogicBase {
        #region Public Variables
        [Header("Settings")]
        [SerializeField] private float m_radius;
        [SerializeField] private float m_boostMultiplier = 3f;

        [Header("Gizmos")]
        [SerializeField] private bool m_gizmosEnabled;
        #endregion

        #region Behaviour Callbacks
        private void OnDrawGizmos() {
            if (!m_gizmosEnabled)
                return;

            var halfReachablePoint = m_weaponCamera.position + m_weaponCamera.forward * m_range;

            Gizmos.DrawSphere(halfReachablePoint, m_radius);
            var hitColliders = Physics.OverlapSphere(halfReachablePoint, m_radius);

            var log = hitColliders.Length > 0 ? "Raycast fired with hit" : "Raycast fired without hit";
            Debug.Log($"{_weapon.name} - {log}");

            foreach (var col in hitColliders) {
                var damageable = col.GetComponent<IDamageable>();

                if (damageable != null) {
                    Gizmos.color = Color.green;
                    Debug.Log($"{_weapon.name} - Damageable detected");
                    continue;
                }

                Gizmos.color = Color.blue;
                Debug.Log($"{_weapon.name} - Collider detected : {col.name}");
            }
        }
        #endregion

        #region Overrides
        public override void Shoot(Ammunition ammunition) {
            var halfReachablePoint = m_weaponCamera.position + m_weaponCamera.forward * m_range;
            Effect(m_effectOrigin.position, halfReachablePoint);

            var hitColliders = Physics.OverlapSphere(halfReachablePoint, m_radius);

            if (hitColliders.Length <= 0)
                return;

            foreach (var col in hitColliders) {
                var damageable = col.GetComponentInParent<IDamageable>();

                if (damageable == null)
                    continue;

                if (!damageable.GetDamage(ammunition.DamageType))
                    continue;

                damageable.ApplyDamage(ammunition.DamageAmount);
            }
        }

        public override void Boost() {
            m_range *= m_boostMultiplier;
            m_radius *= m_boostMultiplier;
        }

        protected override void Effect(Vector3 origin, Vector3 lastPosition) => AudioManager.instance.PlayOneShot(m_soundEventReference, origin);
        #endregion
    }
}