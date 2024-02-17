using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI {
    public class SliderSettingUI : SettingsComponentUI<float> {
        #region Public Variables
        [SerializeField] protected TMP_Text m_valueText;
        [SerializeField] private Slider m_slider;
        #endregion

        protected override void Awake() => m_slider.onValueChanged.AddListener(UpdateText);

        #region Public Methods
        public void SetValueSilently(float value) {
            m_slider.SetValueWithoutNotify(value);
            UpdateText(value);
        }

        public override void Register(System.Action<float> onValueChanged) => m_slider.onValueChanged.AddListener(onValueChanged.Invoke);
        #endregion

        protected virtual void UpdateText(float value) => m_valueText.text = Mathf.FloorToInt(value).ToString();
    }
}