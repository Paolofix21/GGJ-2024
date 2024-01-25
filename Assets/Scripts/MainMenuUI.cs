using Code.LevelSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Code.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        #region Public Variables
        [SerializeField] private Button loadLevel;
        #endregion

        #region Properties
        #endregion

        #region Private Variables
        #endregion

        #region Behaviour Callbacks
        private void Awake()
        {
            loadLevel.onClick.RemoveAllListeners();
        }
        private void Start()
        {
            loadLevel.onClick.AddListener(delegate {
                SceneLoader.LoadScene("Hell", UnityEngine.SceneManagement.LoadSceneMode.Single);
                loadLevel.interactable = false;
            });
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

