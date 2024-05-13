using UnityEngine;
using UnityEngine.SceneManagement;

namespace Miscellaneous {
    public class SpawnOnDestroy : MonoBehaviour {
        #region Public Variables
        [SerializeField] private GameObject m_prefab;
        #endregion

        #region Behaviour Callbacks
        private void OnDestroy() {
            if (!gameObject.scene.isLoaded)
                return;

            var instance = Instantiate(m_prefab, transform.position, transform.rotation);
            SceneManager.MoveGameObjectToScene(instance, gameObject.scene);
        }
        #endregion
    }
}