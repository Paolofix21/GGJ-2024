using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Utilities {
    [DefaultExecutionOrder(-100)]
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour {
        #region Public Variables
        [SerializeField] private bool m_surviveSceneChanged;
        #endregion

        #region Properties
        public static T Singleton { get; private set; }
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            if (Singleton && Singleton != this) {
                Destroy(gameObject);
                return;
            }

            Singleton = this as T;
            OnAfterAwake();

            if (!m_surviveSceneChanged)
                return;

            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy() {
            if (Singleton != this)
                return;

            OnBeforeDestroy();
            Singleton = null;
        }
        #endregion

        #region Public Methods
        public void DieWithScene() => SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
        public void DieWithScene(Scene scene) => SceneManager.MoveGameObjectToScene(gameObject, scene);
        #endregion

        #region Private Methods
        protected virtual void OnAfterAwake() { }
        protected virtual void OnBeforeDestroy() { }
        #endregion
    }
}