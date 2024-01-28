using UnityEngine;

namespace Code.Weapons {

    public class PhysicHitLogic : FiringLogic {
        [Header("Settings")]
        [SerializeField] private float radius = default;

        [Header("Gizmos")]
        [SerializeField] private bool gizmosEnabled = default;

        private void OnDrawGizmos() {
            if (!gizmosEnabled)
                return;

            Vector3 halfReachablePoint = weaponCamera.position + weaponCamera.forward * range;

            Gizmos.DrawSphere(halfReachablePoint, radius);
            Collider[] hitColliders = Physics.OverlapSphere(halfReachablePoint, radius);

            string log = hitColliders.Length > 0 ? "Raycast fired with hit" : "Raycast fired without hit";
            Debug.Log($"{gameObject.name} - {log}");

            foreach (Collider collider in hitColliders) {
                IDamageable damageable = collider.GetComponent<IDamageable>();

                if (damageable != null) {
                    Gizmos.color = Color.green;
                    Debug.Log($"{gameObject.name} - Damageable detected");
                    continue;
                }

                Gizmos.color = Color.blue;
                Debug.Log($"{gameObject.name} - Collider detected : {collider.name}");
            }
        }

        public override void Shoot(Ammunition ammunition) {
            Cooldown(true);

            Vector3 halfReachablePoint = weaponCamera.position + weaponCamera.forward * range;
            Effect(effectOrigin.position, halfReachablePoint);

            Collider[] hitColliders = Physics.OverlapSphere(halfReachablePoint, radius);

            if (hitColliders.Length <= 0)
                return;

            foreach (Collider collider in hitColliders) {
                IDamageable damageable = collider.GetComponent<IDamageable>();

                if (damageable == null)
                    continue;

                if (!damageable.GetDamage(ammunition.GetDamageType()))
                    return;

                damageable.ApplyDamage(ammunition.GetDamageAmount());
            }
        }

        protected override void Effect(Vector3 origin, Vector3 lastPosition) {
            AudioManager.instance.PlayOneShot(soundEventReference, origin);
        }

        public override void Boost() {
            range *= 3;
            radius *= 3;
        }
    }

}