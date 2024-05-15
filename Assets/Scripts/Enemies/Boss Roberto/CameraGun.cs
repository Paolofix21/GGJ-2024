using UnityEngine;

namespace Enemies.BossRoberto {
    public class CameraGun : MonoBehaviour {
        #region Public Variables
        [Header("Prefabs")]
        [SerializeField] private CameraBullet m_bulletPrefab;

        [Header("Settings")]
        [SerializeField] private Transform m_spawnPoint;

        [Header("Settings")]
        [SerializeField] private float m_bulletSpeed = 5f;
        [SerializeField] private float m_damage = 5f;
        #endregion

        #region Public Methods
        public void Shoot() {
            var bullet = Instantiate(m_bulletPrefab, m_spawnPoint.position, transform.rotation);
            bullet.OverrideDamage(m_damage);
            bullet.Launch(m_spawnPoint.forward * m_bulletSpeed);
        }
        #endregion
    }
}