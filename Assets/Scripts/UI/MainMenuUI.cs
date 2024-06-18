using System.Collections;
using Audio;
using Code.Core;
using Code.LevelSystem;
using LanguageSystem.Runtime.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI {
    public class MainMenuUI : MonoBehaviour {
        #region Public Variables
        [SerializeField] private Button m_loadLevel;
        [SerializeField] private Button m_loadSettings;
        [SerializeField] private Button m_quitGame;

        [Space]
        [SerializeField] private SoundSO m_mainMenuSoundtrack;
        [SerializeField] private SoundSO m_explorationSoundTrack;
        [SerializeField, Min(1f)] private float m_crossFadeDuration = 3f;

        [Space]
        [SerializeField] private LocalizedStringRecord m_quitToDesktopText;
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            m_loadLevel.onClick.RemoveAllListeners();
            m_loadSettings.onClick.RemoveAllListeners();
            m_quitGame.onClick.RemoveAllListeners();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            GameEvents.Purge();
        }

        private IEnumerator Start() {
            yield return null;

            AudioManager.Singleton.CrossFadeMusic(m_mainMenuSoundtrack.GetSound(), m_crossFadeDuration);

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
            AudioManager.Singleton.AmbienceFadeTo(1f);
            AudioManager.Singleton.CrossFadeMusic(m_explorationSoundTrack.GetSound(), m_crossFadeDuration);
        }

        private void Quit() => UIManager.Singleton.CallConfirmTask(m_quitToDesktopText.GetString(), QuitGame);
        #endregion
    }
}