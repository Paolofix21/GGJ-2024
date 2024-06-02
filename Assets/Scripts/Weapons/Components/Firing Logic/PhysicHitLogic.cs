using Audio;
using UnityEngine;

namespace Code.Weapons {
    [System.Serializable]
    public class PhysicHitLogic : FiringLogicBase {
        #region Public Variables
        [Header("Settings")]
        [SerializeField] private float m_radius;
        [SerializeField] private float m_boostMultiplier = 3f;
        [SerializeField] private LayerMask m_targetLayers = 11;
        #endregion

        #region Overrides
        public override void Shoot(Ammunition ammunition) {
            var halfReachablePoint = m_weaponCamera.position + m_weaponCamera.forward * m_range;
            Effect(m_effectOrigin.position, halfReachablePoint);

            var hitColliders = Physics.OverlapSphere(halfReachablePoint, m_radius, m_targetLayers);

            if (hitColliders.Length <= 0)
                return;

            foreach (var col in hitColliders) {
                var damageable = col.GetComponentInParent<IDamageable>();

                if (damageable == null)
                    continue;

                if (!damageable.GetDamage(ammunition.DamageType))
                    continue;

                damageable.ApplyDamage(ammunition.DamageAmount, _weapon.gameObject);
            }
        }

        public override void Boost() {
            m_range *= m_boostMultiplier;
            m_radius *= m_boostMultiplier;
        }

        protected override void Effect(Vector3 origin, Vector3 lastPosition) => AudioManager.Singleton.PlayOneShotWorld(m_shootSound.GetSound(), origin, MixerType.SoundFx);
        #endregion
    }
}