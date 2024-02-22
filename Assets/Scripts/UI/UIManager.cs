using UnityEngine;

namespace Code.UI {
    public class UIManager : MonoBehaviour {
        #region Public Variables
        public ConfirmTaskUI ConfirmTaskUIPrefab;
        public SettingsUI SettingsUIPrefab;
        #endregion

        #region Properties
        public static UIManager Singleton;
        #endregion

        #region Private Variables
        private bool _currentStatePauseMenu;
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            if (Singleton && Singleton != this) {
                Destroy(gameObject);
                return;
            }

            Singleton = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy() {
            if (Singleton && Singleton == this)
                Singleton = null;
        }
        #endregion

        #region Public Methods
        public void CallConfirmTask(string textToDisplay, System.Action action) {
            if (ConfirmTaskUIPrefab != null)
                Instantiate(ConfirmTaskUIPrefab).Setup(textToDisplay, action);
        }

        public void CallSettings() => Instantiate(SettingsUIPrefab);
        #endregion
    }
}