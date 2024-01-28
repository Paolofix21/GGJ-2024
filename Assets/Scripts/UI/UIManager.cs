using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace Code.UI
{
    public class UIManager : MonoBehaviour
    {
        #region Public Variables
        public ConfirmTaskUI ConfirmTaskUIPrefab;
        public SettingsUI SettingsUIPrefab;
        public PauseUI PauseUI = default;
        #endregion

        #region Properties
        public static UIManager Singleton;
        #endregion

        #region Private Variables
        private bool currentStatePauseMenuc = default;
        private PauseUI instantiatedPauseMenu = default;
        #endregion

        #region Behaviour Callbacks
        private void Awake()
        {
            if (Singleton && Singleton != this)
            {
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
        private void Start() {
            instantiatedPauseMenu = Instantiate(PauseUI, transform);
            instantiatedPauseMenu.gameObject.SetActive(false);
        }
        #endregion

        #region Public Methods
        public void CallConfirmTask(string textToDisplay, System.Action action)
        {
            if (ConfirmTaskUIPrefab != null) { 
                Instantiate(ConfirmTaskUIPrefab).Setup(textToDisplay, action);
            }
        }
        public void CallSettings()
        {
            Instantiate(SettingsUIPrefab);
        }
        public void CallPauseUI() {
            currentStatePauseMenuc = !currentStatePauseMenuc;
            Time.timeScale = currentStatePauseMenuc ? 0.0f : 1.0f;
            Cursor.lockState = currentStatePauseMenuc ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = currentStatePauseMenuc;
            PauseUI.gameObject.SetActive(currentStatePauseMenuc);
        }
        #endregion

        #region Private Methods
        #endregion

        #region Virtual Methods
        #endregion
    }
}
