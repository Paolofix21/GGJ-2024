using Code.LevelSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI {
    public class MainMenuUI : MonoBehaviour {
        #region Public Variables
        [SerializeField] private Button m_loadLevel;
        [SerializeField] private Button m_loadSettings;
        [SerializeField] private Button m_quitGame;
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            m_loadLevel.onClick.RemoveAllListeners();
            m_loadSettings.onClick.RemoveAllListeners();
            m_quitGame.onClick.RemoveAllListeners();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void Start() {
            AudioManager.instance.PlayMainMenuMusic();

            m_loadLevel.onClick.AddListener(LoadGameScenes);
            m_loadSettings.onClick.AddListener(UIManager.Singleton.CallSettings);
            m_quitGame.onClick.AddListener(Quit);
        }
        #endregion

        #region Private Methods
        private void QuitGame() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
        #endregion

        #region Event Methods
        private void LoadGameScenes() {
            SceneLoader.LoadScenes("Game Scene 01", "Game Scene 01 Waves", "Game Scene 01 UI");
            m_loadLevel.interactable = false;
            AudioManager.instance.ChangeGlobalMusicAmbienceParameter(1);
            AudioManager.instance.PlayExplorationMusic();
        }

        private void Quit() => UIManager.Singleton.CallConfirmTask("Do you really want to return to the desktop?", QuitGame);
        #endregion
    }
}