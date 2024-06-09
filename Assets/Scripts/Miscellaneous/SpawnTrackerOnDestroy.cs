using UnityEngine;
using Weapons.Components;

namespace Miscellaneous {
    public class SpawnTrackerOnDestroy : MonoBehaviour {
        #region Public Variables
        [Header("Prefabs")]
        [SerializeField] private BulletCurvedTrail m_tracker;

        [Header("References")]
        [SerializeField] private Transform m_target;
        #endregion

        #region Behaviour Callbacks
        private void OnDestroy() {
            if (!gameObject.scene.isLoaded)
                return;

            Instantiate(m_tracker, transform.position, transform.rotation).SetDestination(m_target.position);
        }
        #endregion
    }
}