using UnityEngine;

namespace Code.Weapons {

    public class PhysicHitLogic : FiringLogic {
        [SerializeField] private float radius = default;

        private Transform playerCamera = default;

        public override void Shoot(Ammunition ammunition) {
            OnShotFired?.Invoke();
            Cooldown(true);

            Vector3 halfReachablePoint = (playerCamera.forward * range) / 2.0f;

            Collider[] hitColliders = Physics.OverlapSphere(halfReachablePoint, radius);

            if (hitColliders.Length <= 0)
                return;

            foreach (Collider collider in hitColliders) {
                IDamageable damageable = collider.GetComponent<IDamageable>();

                if(damageable == null) 
                    continue;

                if (!damageable.GetDamage(ammunition.GetDamageType()))
                    return;

                damageable.ApplyDamage(ammunition.GetDamageAmount());
            }
        }
    }

}