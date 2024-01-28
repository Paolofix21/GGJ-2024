using UnityEngine;
using Weapons.Components;

namespace Code.Weapons {

    public class SingleBulletLogic : FiringLogic {
        [SerializeField] private BulletTrail bullet = default;

        [Header("Gizmos")]
        [SerializeField] private bool gizmosEnabled = default;

        private void OnDrawGizmos() {
            if (!gizmosEnabled)
                return;

            Gizmos.DrawLine(weaponCamera.position, weaponCamera.position + weaponCamera.forward * range);
            Ray ray = new Ray(weaponCamera.position, weaponCamera.forward);

            string log = Physics.Raycast(ray, out RaycastHit hitInfo, range) ? "Raycast fired with hit" : "Raycast fired without hit";

            Debug.Log($"{gameObject.name} - {log}");

            if (hitInfo.collider == null) {
                Gizmos.color = Color.red;
                Debug.Log($"{gameObject.name} - No collider detected");
                return;
            }

            Gizmos.color = Color.blue;
            Debug.Log($"{gameObject.name} -Collider detected");

            IDamageable damageable = hitInfo.collider.GetComponent<IDamageable>();

            if (damageable != null) {
                Gizmos.color = Color.green;
                Debug.Log($"{gameObject.name} - Damageable detected");
            }
        }

        public override void Shoot(Ammunition ammunition) {
            Cooldown(true);

            Vector3 reachablePoint = weaponCamera.position + weaponCamera.forward * range;
            Ray ray = new Ray(weaponCamera.position, weaponCamera.forward);

            Effect(reachablePoint);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, range)) {
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

        protected override void Effect(Vector3 position) {
            AudioManager.instance.PlayOneShot(soundEventReference, effectOrigin.position);
            BulletTrail bulletTrail = Instantiate(bullet, effectOrigin.position, Quaternion.identity);
            bulletTrail.SetDestination(position);
        }
    }

}