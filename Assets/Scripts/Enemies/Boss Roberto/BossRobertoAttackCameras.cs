using System.Collections.Generic;
using UnityEngine;

namespace Enemies.BossRoberto {
    public class BossRobertoAttackCameras : MonoBehaviour {
        #region Public Variables
        [Header("References")]
        [SerializeField] private List<CameraGun> m_cameras = new();

        [Header("Settings")]
        [SerializeField] private float m_errorRange = 1.5f;
        #endregion

        #region Behaviour Callbacks
        private void Awake() => DeactivateCameras();
        #endregion

        #region Public Methods
        public void ActivateCameras() => m_cameras.ForEach(c => c.gameObject.SetActive(true));
        public void DeactivateCameras() => m_cameras.ForEach(c => c.gameObject.SetActive(false));

        public void ShootAt(Transform target) {
            Debug.Log("Shooting...\n");
            m_cameras.ForEach(c => {
                var point = target.position + Random.onUnitSphere * m_errorRange;
                c.transform.LookAt(point);
                c.Shoot();
            });
        }
        #endregion
    }
}