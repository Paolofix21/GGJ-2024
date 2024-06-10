using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI {
    public class ToolbarSettingUI : SettingsComponentUI<int> {
        #region Public Variables
        [SerializeField] private List<Toggle> m_toggles = new();
        [SerializeField] private ToggleGroup m_group;
        #endregion

        #region Private Variables
        private event System.Action<int> _onValueChanged;
        #endregion

        #region Behaviour Callbacks
        protected override void Awake() {
            foreach (var toggle in m_toggles) {
                toggle.onValueChanged.AddListener(OnToggleChanged);
                m_group.RegisterToggle(toggle);
            }

            m_group.EnsureValidState();
        }
        #endregion

        #region Public Methods
        public void SetValueSilently(int index) {
            if (!didAwake)
                Awake();

            m_toggles[index].SetIsOnWithoutNotify(true);
            m_group.NotifyToggleOn(m_toggles[index], false);
        }

        public override void Register(System.Action<int> onValueChanged) => _onValueChanged += onValueChanged.Invoke;
        #endregion

        #region Private Methods
        private void OnToggleChanged(bool isOn) {
            var toggle = m_group.GetFirstActiveToggle();
            var index = m_toggles.IndexOf(toggle);
            _onValueChanged?.Invoke(index);
        }
        #endregion
    }
}