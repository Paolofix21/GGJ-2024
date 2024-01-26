using UnityEngine;

namespace Code.Weapons {

    public class ZoneBulletLogic : FiringLogic {
        [SerializeField] private float radius = default;

        private Transform playerCamera = default;

        public override void Shoot(Ammunition ammunition) {
            if (Physics.CapsuleCast(playerCamera.position, playerCamera.forward * range, radius, playerCamera.forward, out RaycastHit hitInfo)) {

                if (hitInfo.collider == null)
                    return;

                Physics.OverlapSphere(hitInfo.point, radius);

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