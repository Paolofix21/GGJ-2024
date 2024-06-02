using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SteamIntegration.UI {
    public class SteamOnlineUi : UIBehaviour {
        #region Public Variables
        [SerializeField] private TMP_Text m_userNameText;
        #endregion

        #region Behaviour Callbacks
        protected override void Awake() => m_userNameText.text = SteamManager.Singleton.User.Name;
        #endregion
    }
}