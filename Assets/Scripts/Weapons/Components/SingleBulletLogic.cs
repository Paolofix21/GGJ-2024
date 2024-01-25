using Code.Weapon;
using UnityEngine;

namespace Code.Weapons {

    public class SingleBulletLogic : FiringLogic {
        private Camera playerCamera = default;

        public override void Shoot(Ammunition ammunition) {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

            if(Physics.Raycast(ray, out RaycastHit hitInfo, range)) {
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