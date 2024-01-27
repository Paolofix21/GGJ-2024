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
        #endregion

        #region Properties
        #endregion

        #region Private Variables
        private void Awake()
        {
            returnMainMenu.onClick.RemoveAllListeners();
        }
        private void Start()
        {
            returnMainMenu.onClick.AddListener(delegate { SceneLoader.LoadScene("MainMenu", UnityEngine.SceneManagement.LoadSceneMode.Single); });
        }
        #endregion

        #region Behaviour Callbacks
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        #endregion

        #region Virtual Methods
        #endregion
    }
}
