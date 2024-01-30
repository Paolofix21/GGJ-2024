using System.Collections.Generic;
using UnityEngine;

namespace Code.EnemySystem.Boss {
    public class BossAttackFireBalls : MonoBehaviour {
        #region Public Variables
        [Header("Prefabs")]
        [SerializeField] private FireBall m_fireBallPrefab;

        [Header("Prefabs")]
        [SerializeField] private float m_shootSpeed = 6f;

        [Header("References")]
        [SerializeField] private List<Transform> m_spawnPoints = new();
        #endregion

        #region Public Methods
        public void Shoot() {
            foreach (var spawnPoint in m_spawnPoints) {
                var ball = Instantiate(m_fireBallPrefab, spawnPoint.position, spawnPoint.rotation);
                ball.Shoot(spawnPoint.forward * m_shootSpeed);
            }
        }
        #endregion
    }
}