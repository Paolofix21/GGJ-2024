using UnityEngine;
using UnityEngine.UI;

namespace Code.UI {
    public class ToggleSettingUI : SettingsComponentUI<bool> {
        #region Public Variables
        [SerializeField] private Toggle m_toggle;
        #endregion

        #region Public Methods
        public void SetValueSilently(bool value) => m_toggle.SetIsOnWithoutNotify(value);
        public override void Register(System.Action<bool> onValueChanged) => m_toggle.onValueChanged.AddListener(onValueChanged.Invoke);
        #endregion
    }
}