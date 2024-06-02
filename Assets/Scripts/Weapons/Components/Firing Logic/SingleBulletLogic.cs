using Audio;
using UnityEngine;
using Weapons.Components;

namespace Code.Weapons {
    public class SingleBulletLogic : FiringLogicBase {
        #region Public Variables
        [SerializeField] private BulletTrail m_bullet;
        [SerializeField] private GameObject m_hitParticle;
        [SerializeField] private float m_boostMultiplier = 3f;
        #endregion

        #region Overrides
        public override void Shoot(Ammunition ammunition) {
            var reachablePoint = m_weaponCamera.position + m_weaponCamera.forward * m_range;
            var ray = new Ray(m_weaponCamera.position, m_weaponCamera.forward);

            Effect(m_effectOrigin.position, reachablePoint);

            if (!Physics.Raycast(ray, out var hitInfo, m_range))
                return;

            if (hitInfo.collider == null)
                return;

            Object.Instantiate(m_hitParticle, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));

            var damageable = hitInfo.collider.GetComponentInParent<IDamageable>();

            if (damageable == null)
                return;

            if (!damageable.GetDamage(ammunition.DamageType))
                return;

            damageable.ApplyDamage(ammunition.DamageAmount, _weapon.gameObject);
        }

        public override void Boost() => m_range *= m_boostMultiplier;

        protected override void Effect(Vector3 origin, Vector3 lastPosition) {
            AudioManager.Singleton.PlayOneShotWorld(m_shootSound.GetSound(), origin, MixerType.SoundFx);
            var bulletTrail = Object.Instantiate(m_bullet, origin, Quaternion.identity);
            bulletTrail.SetDestination(lastPosition);
        }
        #endregion
    }
}