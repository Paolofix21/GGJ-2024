using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace Code.UI
{
    public class ConfirmTaskUI : MonoBehaviour
    {
        #region Public Variables   
        [SerializeField] private TMP_Text m_textToDisplay;
        [SerializeField] private Button m_discardButton;
        [SerializeField] private Button m_confirmButton;
        [SerializeField] private Animator m_Animator;
        #endregion

        #region Properties
        #endregion

        #region Private Variables
        #endregion

        #region Behaviour Callbacks
        private void Awake()
        {
            m_discardButton.onClick.RemoveAllListeners();
            m_confirmButton.onClick.RemoveAllListeners();
        }
        private void Start()
        {
            m_discardButton.onClick.AddListener(PlayAndDestroy);
        }
        #endregion

        #region Public Methods
        public void Setup(string textToDisplay, System.Action action)
        {
            m_textToDisplay.text = textToDisplay;
            if(action != null)
            {
                m_confirmButton.onClick.AddListener(action.Invoke);
            }
        }
        #endregion

        #region Private Methods
        private async void PlayAndDestroy()
        {
            m_Animator.SetTrigger("go");
            await Task.Delay(1000);
            Destroy(gameObject);
        }
        #endregion

        #region Virtual Methods
        #endregion
    }
}
