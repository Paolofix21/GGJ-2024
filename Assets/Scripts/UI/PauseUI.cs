using Code.LevelSystem;
using Audio;
using Code.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI {
    public class PauseUI : MonoBehaviour {
        #region Public Variables
        [Header("Settings")]
        [SerializeField] private float m_musicAttenuation = .5f;
        [SerializeField] private float m_musicAttenuationTime = 1f;

        [Header("References")]
        [SerializeField] private Button resume;
        [SerializeField] private Button returnMainMenu;
        [SerializeField] private Button m_loadSettings;
        [SerializeField] private Button m_quitSettings;
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            GameEvents.OnPauseStatusChanged += ToggleShow;
            gameObject.SetActive(false);
        }

        private void OnEnable() {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (didStart)
                AudioManager.Singleton.AttenuateMusic(m_musicAttenuation, m_musicAttenuationTime);
        }

        private void Start() {
            returnMainMenu.onClick.AddListener(BackToMenu);

            resume.onClick.AddListener(GameEvents.Resume);

            m_loadSettings.onClick.AddListener(UIManager.Singleton.CallSettings);
            m_quitSettings.onClick.AddListener(Quit);
        }

        private void OnDisable() {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            AudioManager.Singleton?.AttenuateMusic(1f, .25f);
        }

        private void OnDestroy() {
            GameEvents.OnPauseStatusChanged -= ToggleShow;
            GameEvents.Resume();
        }
        #endregion

        #region Public Methods
        public void QuitGame() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
        #endregion

        #region Private Methods
        private void ToggleShow(bool pause) => gameObject.SetActive(pause);
        #endregion

        #region Event Methods
        private void BackToMenu() {
            gameObject.SetActive(false);
            SceneLoader.LoadScene("MainMenu", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }

        private void Quit() => UIManager.Singleton.CallConfirmTask("Do you really want to return to the desktop?", QuitGame);
        #endregion
    }
}