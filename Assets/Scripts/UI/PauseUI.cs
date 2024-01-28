using Code.LevelSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Code.UI
{
    public class PauseUI : MonoBehaviour
    {
        #region Public Variables  
        [SerializeField] private Button returnMainMenu;
        [SerializeField] private Button m_loadSettings;
        [SerializeField] private Button m_quitSettings;
        #endregion

        #region Properties
        #endregion

        #region Private Variables
        #endregion

        #region Behaviour Callbacks
        private void Awake()
        {
            returnMainMenu.onClick.RemoveAllListeners();
            m_loadSettings.onClick.RemoveAllListeners();
            m_quitSettings.onClick.RemoveAllListeners();
            UIManager.Singleton.PauseUI = this;
        }
        private void Start()
        {
            returnMainMenu.onClick.AddListener(delegate {
                gameObject.SetActive(false); 
                Time.timeScale = 1.0f; 
                SceneLoader.LoadScene("MainMenu", UnityEngine.SceneManagement.LoadSceneMode.Single); 
            });

            m_loadSettings.onClick.AddListener(UIManager.Singleton.CallSettings);
            m_quitSettings.onClick.AddListener(delegate {
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
