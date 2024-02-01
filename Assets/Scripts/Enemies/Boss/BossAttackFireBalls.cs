using System.Collections.Generic;
using UnityEngine;

namespace Code.EnemySystem.Boss {
    public class BossAttackFireBalls : MonoBehaviour {
        #region Public Variables
        [Header("Prefabs")]
        [SerializeField] private FireBall m_fireBallPrefab;

        [Header("Settings")]
        [SerializeField] private float m_shootSpeed = 6f;

        [Header("References")]
        [SerializeField] private List<Transform> m_spawnPoints = new();
        [SerializeField] private List<Transform> m_spawnPointsOdd = new();
        #endregion

        #region Private Variables
        private bool _odd;
        #endregion

        #region Behaviour Callbacks
        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            m_spawnPoints.ForEach(sp => Gizmos.DrawRay(sp.position, sp.forward * 3f));
            Gizmos.color = Color.blue;
            m_spawnPointsOdd.ForEach(sp => Gizmos.DrawRay(sp.position, sp.forward * 3f));
            Gizmos.color = Color.white;
        }
        #endregion

        #region Public Methods
        public void Shoot() {
            var points = _odd ? m_spawnPointsOdd : m_spawnPoints;

            foreach (var spawnPoint in points) {
                var ball = Instantiate(m_fireBallPrefab, spawnPoint.position, spawnPoint.rotation);
                ball.Shoot(spawnPoint.forward * m_shootSpeed);
            }

            _odd = !_odd;
        }

        public void ShootAt(Transform target) {
            var points = _odd ? m_spawnPointsOdd : m_spawnPoints;

            foreach (var spawnPoint in points) {
                var ball = Instantiate(m_fireBallPrefab, spawnPoint.position, spawnPoint.rotation);
                ball.Shoot(spawnPoint.forward * m_shootSpeed);
                ball.SetHomingTarget(target);
            }

            _odd = !_odd;
        }
        #endregion
    }
}