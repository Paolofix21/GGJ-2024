using System.Threading.Tasks;
using LanguageSystem.Runtime.Components;
using LanguageSystem.Runtime.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI {
    public class ConfirmTaskUI : MonoBehaviour {
        #region Public Variables
        [SerializeField] private LocalizeText m_textToDisplay;
        [SerializeField] private Button m_discardButton;
        [SerializeField] private Button m_confirmButton;
        [SerializeField] private Animator m_Animator;
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            m_discardButton.onClick.RemoveAllListeners();
            m_confirmButton.onClick.RemoveAllListeners();
        }

        private void Start() {
            m_discardButton.onClick.AddListener(delegate {
                m_discardButton.interactable = false;
                PlayAndDestroy();
            });
        }
        #endregion

        #region Public Methods
        public void Setup(LocalizedString textToDisplay, System.Action action) {
            m_textToDisplay.OverrideLocalizedString(textToDisplay);
            if (action != null) {
                m_confirmButton.onClick.AddListener(action.Invoke);
            }
        }
        #endregion

        #region Private Methods
        private async void PlayAndDestroy() {
            m_Animator.SetTrigger("go");
            await Task.Delay(1000);
            Destroy(gameObject);
        }
        #endregion
    }
}