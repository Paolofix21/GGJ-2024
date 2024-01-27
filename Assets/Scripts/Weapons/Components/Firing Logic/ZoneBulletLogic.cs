using UnityEngine;

namespace Code.Weapons {

    public class ZoneBulletLogic : FiringLogic {
        [SerializeField] private float radius = default;
        [SerializeField] private int interestedPoints = default;

        private Transform playerCamera = default;

        public override void Shoot(Ammunition ammunition) {
            OnShotFired?.Invoke();
            Cooldown(true);

            Vector3 lastReachablePoint = playerCamera.forward * range;

            for(int i = 0; i < interestedPoints; i++) {
                float randomX = Random.Range(lastReachablePoint.x - radius, lastReachablePoint.x + radius);
                float randomY = Random.Range(lastReachablePoint.y - radius, lastReachablePoint.y + radius);
                Vector3 randomReachablePoint = new Vector3(randomX, randomY, lastReachablePoint.z);

                if(Physics.Linecast(playerCamera.position, randomReachablePoint, out RaycastHit hitInfo)) {
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