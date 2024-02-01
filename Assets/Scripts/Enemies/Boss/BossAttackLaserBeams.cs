using System.Collections.Generic;
using UnityEngine;

namespace Code.EnemySystem.Boss {
    public class BossAttackLaserBeams : MonoBehaviour {
        #region Public Variables
        [Header("Prefabs")]
        [SerializeField] private LaserBeam m_laserBeamPrefab;

        [Header("References")]
        [SerializeField] private Transform m_defaultTarget;
        [SerializeField] private List<Transform> m_eyes = new();
        #endregion

        #region Behaviour Callbacks
        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            m_eyes.ForEach(sp => Gizmos.DrawRay(sp.position, sp.forward * 3f));
            Gizmos.color = Color.white;
        }
        #endregion

        #region Public Methods
        public void Shoot(float duration) {
            foreach (var spawnPoint in m_eyes) {
                var beam = Instantiate(m_laserBeamPrefab, spawnPoint.position, spawnPoint.rotation);
                beam.AnchorTo(spawnPoint);
                beam.SetTime(duration);
            }
        }

        public void ShootAt(float duration, Transform target = null) {
            target ??= m_defaultTarget;

            foreach (var spawnPoint in m_eyes) {
                var beam = Instantiate(m_laserBeamPrefab, spawnPoint.position, spawnPoint.rotation);
                beam.AnchorTo(spawnPoint);
                beam.SetTime(duration);
                beam.Track(target);
            }
        }
        #endregion
    }
}