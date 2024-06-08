using Audio;
using UnityEngine;
using Weapons.Components;

namespace Code.Weapons {
    public class ZoneBulletLogic : FiringLogicBase {
        #region Public Variables
        [SerializeField] private BulletTrail bullet = default;
        [SerializeField] private GameObject hitParticle = default;
        [SerializeField] private float m_boostMultiplier = 3f;
        [SerializeField] private int m_spreadSeed = 0;

        [Header("Settings")]
        [SerializeField][Range(0f, 5f)] private float radius = default;
        [SerializeField] private int interestedPoints = default;
        [SerializeField] private bool doesCollateral = default;
        #endregion

        #region Overrides
        public override void Shoot(Ammunition ammunition) {
            var lastReachablePoint = m_weaponCamera.position + m_weaponCamera.forward * m_range;

            Random.InitState(m_spreadSeed);

            for (var i = 0; i < interestedPoints; i++) {
                var remappedRandomPoint = m_weaponCamera.TransformPoint((Vector3)Random.insideUnitCircle * radius) - m_weaponCamera.position;
                var randomReachablePoint = lastReachablePoint + remappedRandomPoint;

                Effect(m_effectOrigin.position, randomReachablePoint);

                bool didHit;
                var hitsInfos = new RaycastHit[4];

                if (doesCollateral) {
                    var dir = randomReachablePoint - m_weaponCamera.position;
                    didHit = Physics.RaycastNonAlloc(m_weaponCamera.position, dir.normalized, hitsInfos, dir.magnitude) > 0;
                }
                else {
                    didHit = Physics.Linecast(m_weaponCamera.position, randomReachablePoint, out var hitInfo);
                    hitsInfos = new[] { hitInfo };
                }

                if (!didHit)
                    continue;

                foreach (var hitInfo in hitsInfos) {
                    if (hitInfo.collider == null)
                        continue;

                    Object.Instantiate(hitParticle, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));

                    var damageable = hitInfo.collider.GetComponentInParent<IDamageable>();

                    if (damageable == null)
                        continue;

                    if (!damageable.GetDamage(ammunition.DamageType))
                        continue;

                    damageable.ApplyDamage(ammunition.DamageAmount, _weapon.gameObject);
                    HitTarget(damageable);
                }
            }
        }

        public override void Boost() => m_range *= m_boostMultiplier;

        protected override void Effect(Vector3 origin, Vector3 lastPosition) {
            AudioManager.Singleton.PlayOneShotWorld(m_shootSound.GetSound(), origin, MixerType.SoundFx);
            var bulletTrail = Object.Instantiate(bullet, origin, Quaternion.identity);
            bulletTrail.SetDestination(lastPosition);
        }
        #endregion
    }

}