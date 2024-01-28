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
        }
        private void Start()
        {
            returnMainMenu.onClick.AddListener(delegate { SceneLoader.LoadScene("MainMenu", UnityEngine.SceneManagement.LoadSceneMode.Single); });
            m_loadSettings.onClick.AddListener(UIManager.Singleton.CallSettings);
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        #endregion

        #region Virtual Methods
        #endregion
    }
}
