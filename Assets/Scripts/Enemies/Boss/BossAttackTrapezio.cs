using UnityEngine;

namespace Code.EnemySystem.Boss {
    public class BossAttackTrapezio : MonoBehaviour {
        #region Public Variables
        [Header("Prefabs")]
        [SerializeField] private FireBall m_fireBallPrefab;

        [Header("Settings")]
        [SerializeField] private float m_shootSpeed = 6f;

        [Header("References")]
        [SerializeField] private Transform m_spawnPoint;
        #endregion

        #region Behaviour Callbacks
        private void OnDrawGizmos() {
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(m_spawnPoint.position, m_spawnPoint.forward * 3f);
            Gizmos.color = Color.white;
        }
        #endregion

        #region Public Methods
        public void Shoot() {
            var ball = Instantiate(m_fireBallPrefab, m_spawnPoint.position, m_spawnPoint.rotation);
            ball.Shoot(m_spawnPoint.forward * m_shootSpeed);
        }

        public void ShootAt(Transform target) {
            var ball = Instantiate(m_fireBallPrefab, m_spawnPoint.position, m_spawnPoint.rotation);
            ball.Shoot(m_spawnPoint.forward * m_shootSpeed);
            ball.SetHomingTarget(target);
        }
        #endregion
    }
}