using UnityEngine;
using Weapons.Components;

namespace Code.Weapons {

    public class ZoneBulletLogic : FiringLogic {
        [SerializeField] private BulletTrail bullet = default;

        [Header("Settings")]
        [SerializeField][Range(0f, 1f)] private float radius = default;
        [SerializeField] private int interestedPoints = default;

        [Header("Gizmos")]
        [SerializeField] private bool gizmosEnabled = default;

        private void OnDrawGizmos() {
            if (!gizmosEnabled)
                return;

            Vector3 lastReachablePoint = weaponCamera.position + weaponCamera.forward * range;

            for (int i = 0; i < interestedPoints; i++) {
                Vector3 randomReachablePoint = lastReachablePoint + (Vector3)Random.insideUnitCircle * radius;

                Gizmos.DrawLine(weaponCamera.position, randomReachablePoint);

                string log = Physics.Linecast(weaponCamera.position, randomReachablePoint, out RaycastHit hitInfo) ? "Raycast fired with hit" : "Raycast fired without hit";

                Debug.Log($"{gameObject.name} - {log}");

                if (hitInfo.collider == null) {
                    Gizmos.color = Color.red;
                    Debug.Log($"{gameObject.name} - No collider detected");
                    return;
                }

                Gizmos.color = Color.blue;
                Debug.Log($"{gameObject.name} - Collider detected");

                IDamageable damageable = hitInfo.collider.GetComponent<IDamageable>();

                if (damageable != null) {
                    Gizmos.color = Color.green;
                    Debug.Log($"{gameObject.name} - Damageable detected");
                }

            }
        }

        public override void Shoot(Ammunition ammunition) {
            Cooldown(true);

            Vector3 lastReachablePoint = weaponCamera.position + weaponCamera.forward * range;

            for (int i = 0; i < interestedPoints; i++) {
                Vector3 randomReachablePoint = lastReachablePoint + (Vector3)Random.insideUnitCircle * radius;

                Effect(effectOrigin.position, randomReachablePoint);

                if (Physics.Linecast(weaponCamera.position, randomReachablePoint, out RaycastHit hitInfo)) {
                    if (hitInfo.collider == null)
                        return;

                    IDamageable damageable = hitInfo.collider.GetComponent<IDamageable>();

                    if (damageable == null)
                        return;

                    if (!damageable.GetDamage(ammunition.GetDamageType()))
                        return;

                    damageable.ApplyDamage(ammunition.GetDamageAmount());
                }
            }
        }

        protected override void Effect(Vector3 origin, Vector3 lastPosition) {
            AudioManager.instance.PlayOneShot(soundEventReference, origin);
            BulletTrail bulletTrail = Instantiate(bullet, origin, Quaternion.identity);
            bulletTrail.SetDestination(lastPosition);
        }

        public override void Boost() {
            range *= 3;
        }
    }

}