using UnityEngine;
using Weapons.Components;

namespace Code.Weapons {
    public class ZoneBulletLogic : FiringLogicBase {
        #region Public Variables
        [SerializeField] private BulletTrail bullet = default;
        [SerializeField] private GameObject hitParticle = default;
        [SerializeField] private float m_boostMultiplier = 3f;

        [Header("Settings")]
        [SerializeField][Range(0f, 1f)] private float radius = default;
        [SerializeField] private int interestedPoints = default;

        [Header("Gizmos")]
        [SerializeField] private bool gizmosEnabled = default;
        #endregion

        #region Behaviour Callbacks
        private void OnDrawGizmos() {
            if (!gizmosEnabled)
                return;

            var lastReachablePoint = m_weaponCamera.position + m_weaponCamera.forward * m_range;

            for (var i = 0; i < interestedPoints; i++) {
                Vector3 randomReachablePoint = lastReachablePoint + (Vector3)Random.insideUnitCircle * radius;

                Gizmos.DrawLine(m_weaponCamera.position, randomReachablePoint);

                var log = Physics.Linecast(m_weaponCamera.position, randomReachablePoint, out RaycastHit hitInfo) ? "Raycast fired with hit" : "Raycast fired without hit";

                Debug.Log($"{_weapon.name} - {log}");

                if (hitInfo.collider == null) {
                    Gizmos.color = Color.red;
                    Debug.Log($"{_weapon.name} - No collider detected");
                    return;
                }

                Gizmos.color = Color.blue;
                Debug.Log($"{_weapon.name} - Collider detected");

                var damageable = hitInfo.collider.GetComponent<IDamageable>();

                if (damageable != null) {
                    Gizmos.color = Color.green;
                    Debug.Log($"{_weapon.name} - Damageable detected");
                }

            }
        }
        #endregion

        #region Overrides
        public override void Shoot(Ammunition ammunition) {
            var lastReachablePoint = m_weaponCamera.position + m_weaponCamera.forward * m_range;

            for (var i = 0; i < interestedPoints; i++) {
                Vector3 randomReachablePoint = lastReachablePoint + (Vector3)Random.insideUnitCircle * radius;

                Effect(m_effectOrigin.position, randomReachablePoint);

                if (Physics.Linecast(m_weaponCamera.position, randomReachablePoint, out RaycastHit hitInfo)) {
                    if (hitInfo.collider == null)
                        return;

                    Object.Instantiate(hitParticle, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));

                    var damageable = hitInfo.collider.GetComponentInParent<IDamageable>();

                    if (damageable == null)
                        return;

                    if (!damageable.GetDamage(ammunition.DamageType))
                        return;

                    damageable.ApplyDamage(ammunition.DamageAmount);
                }
            }
        }

        public override void Boost() => m_range *= m_boostMultiplier;

        protected override void Effect(Vector3 origin, Vector3 lastPosition) {
            AudioManager.instance.PlayOneShot(m_soundEventReference, origin);
            var bulletTrail = Object.Instantiate(bullet, origin, Quaternion.identity);
            bulletTrail.SetDestination(lastPosition);
        }
        #endregion
    }

}