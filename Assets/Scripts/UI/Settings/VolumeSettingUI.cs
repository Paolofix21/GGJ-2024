using UnityEngine;
using UnityEngine.UI;

namespace Code.UI {
    public class VolumeSettingUI : SliderSettingUI {
        #region Public Variables
        [SerializeField] private Toggle m_toggle;
        #endregion

        #region Public Methods
        public void SetMuteSilently(bool value) => m_toggle.SetIsOnWithoutNotify(value);
        public void RegisterMute(System.Action<bool> onValueChanged) => m_toggle.onValueChanged.AddListener(onValueChanged.Invoke);
        #endregion

        protected override void UpdateText(float value) => m_valueText.text = Mathf.FloorToInt(value * 100f).ToString();
    }
}