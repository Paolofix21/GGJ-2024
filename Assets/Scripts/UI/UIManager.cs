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
        #endregion

        #region Properties
        public static UIManager Singleton;
        #endregion

        #region Private Variables
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
        #endregion

        #region Public Methods
        public void CallConfirmTask(string textToDisplay, System.Action action)
        {
            if (ConfirmTaskUIPrefab != null) { 
                Instantiate(ConfirmTaskUIPrefab).Setup(textToDisplay, action);
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Virtual Methods
        #endregion
    }
}
