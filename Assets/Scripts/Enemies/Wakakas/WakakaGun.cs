using UnityEngine;

namespace Code.EnemySystem.Wakakas {
    public class WakakaGun : MonoBehaviour {
        #region Public Variables
        [Header("Prefabs")]
        [SerializeField] private WakakaBullet m_bulletPrefab;

        [Header("Settings")]
        [SerializeField] private Transform m_spawnPoint;

        [Header("Settings")]
        [SerializeField] private float m_fireRate = 1.5f;
        [SerializeField] private float m_bulletSpeed = 5f;
        [SerializeField] private float m_damage = 5f;
        #endregion

        #region Behaviour Callbacks
        private void Awake() => Invoke(nameof(Shoot), m_fireRate);

        private void OnDestroy() => CancelInvoke();
        #endregion

        #region Private Methods
        private void Shoot() {
            var bullet = Instantiate(m_bulletPrefab, m_spawnPoint.position, transform.rotation);
            bullet.OverrideDamage(m_damage);
            bullet.Launch(transform.forward * m_bulletSpeed);

            Invoke(nameof(Shoot), m_fireRate);
        }
        #endregion
    }
}