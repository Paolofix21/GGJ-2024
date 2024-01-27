using Unity.Mathematics;
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

            Vector3 halfReachablePoint = playerCamera.position + playerCamera.forward * range;

            Gizmos.DrawSphere(halfReachablePoint, radius);
            Collider[] hitColliders = Physics.OverlapSphere(halfReachablePoint, radius);

            string log = hitColliders.Length > 0 ? "Raycast fired with hit" : "Raycast fired without hit";
            Debug.Log(log);

            foreach (Collider collider in hitColliders) {
                IDamageable damageable = collider.GetComponent<IDamageable>();

                if (damageable != null) {
                    Gizmos.color = Color.green;
                    Debug.Log("Damageable detected");
                    continue;
                }

                Gizmos.color = Color.blue;
                Debug.Log($"Collider detected : {collider.name}");
            }
        }

        public override void Shoot(Ammunition ammunition) {
            OnShotFired?.Invoke();
            Cooldown(true);

            Vector3 halfReachablePoint = (playerCamera.forward * range) / 2.0f;

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
    }

}