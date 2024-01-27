using UnityEngine;

namespace Code.Weapons {

    public class ZoneBulletLogic : FiringLogic {
        [Header("Settings")]
        [SerializeField] private float radius = default;
        [SerializeField] private int interestedPoints = default;

        [Header("Gizmos")]
        [SerializeField] private bool gizmosEnabled = default;

        private void OnDrawGizmos() {
            if (!gizmosEnabled)
                return;

            Vector3 lastReachablePoint = playerCamera.forward * range;

            for (int i = 0; i < interestedPoints; i++) {
                float randomX = Random.Range(lastReachablePoint.x - radius, lastReachablePoint.x + radius);
                float randomY = Random.Range(lastReachablePoint.y - radius, lastReachablePoint.y + radius);
                Vector3 planeVector = new Vector3(randomX, randomY, 0).normalized;
                Vector3 randomReachablePoint = lastReachablePoint + planeVector;

                Gizmos.DrawLine(playerCamera.position, playerCamera.position + randomReachablePoint);

                string log = Physics.Linecast(playerCamera.position, randomReachablePoint, out RaycastHit hitInfo) ? "Raycast fired with hit" : "Raycast fired without hit";

                Debug.Log(log);

                if (hitInfo.collider == null) {
                    Gizmos.color = Color.red;
                    Debug.Log("No collider detected");
                    return;
                }

                Gizmos.color = Color.blue;
                Debug.Log("Collider detected");

                IDamageable damageable = hitInfo.collider.GetComponent<IDamageable>();

                if (damageable != null) {
                    Gizmos.color = Color.green;
                    Debug.Log("Damageable detected");
                }

            }
        }

        public override void Shoot(Ammunition ammunition) {
            OnShotFired?.Invoke();
            Cooldown(true);

            Vector3 lastReachablePoint = playerCamera.forward * range;

            for (int i = 0; i < interestedPoints; i++) {
                float randomX = Random.Range(lastReachablePoint.x - radius, lastReachablePoint.x + radius);
                float randomY = Random.Range(lastReachablePoint.y - radius, lastReachablePoint.y + radius);
                Vector3 planeVector = new Vector3(randomX, randomY, 0).normalized;
                Vector3 randomReachablePoint = lastReachablePoint + planeVector;

                if (Physics.Linecast(playerCamera.position, randomReachablePoint, out RaycastHit hitInfo)) {
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
    }

}