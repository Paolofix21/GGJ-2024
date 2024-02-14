using Code.LevelSystem;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace Code.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        #region Public Variables
        [SerializeField] private Button m_loadLevel;
        [SerializeField] private Button m_loadSettings;
        [SerializeField] private Button m_quitGame;
        #endregion

        #region Properties
        #endregion

        #region Private Variables
        #endregion

        #region Behaviour Callbacks
        private void Awake()
        {
            m_loadLevel.onClick.RemoveAllListeners();
            m_loadSettings.onClick.RemoveAllListeners();
            m_quitGame.onClick.RemoveAllListeners();
        }
        private void Start()
        {
            AudioManager.instance.PlayMainMenuMusic();

            m_loadLevel.onClick.AddListener(delegate {
                SceneLoader.LoadScenes("Game Scene 01", "Game Scene 01 Waves", "Game Scene 01 UI");
                m_loadLevel.interactable = false;
                AudioManager.instance.ChangeGlobalMusicAmbienceParameter(1);
                AudioManager.instance.PlayExplorationMusic();
            });
            m_loadSettings.onClick.AddListener(UIManager.Singleton.CallSettings);
            m_quitGame.onClick.AddListener(delegate {
                UIManager.Singleton.CallConfirmTask("Do you really want to return to the desktop?", QuitGame); 
            });
        }
        #endregion

        #region Public Methods
        public void QuitGame()
        {
            Application.Quit();
        }
        #endregion

        #region Private Methods
        #endregion

        #region Virtual Methods
        #endregion
    }
}

