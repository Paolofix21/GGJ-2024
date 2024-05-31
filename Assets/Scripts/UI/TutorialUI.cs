using Code.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Code.UI {
    public sealed class TutorialUI : UIBehaviour {
        #region Public Variables
        [SerializeField, Min(0.1f)] private float m_progressQuickness = .5f;
        [SerializeField] private Image m_filler;

        [Space]
        [SerializeField] private InputActionReference m_spaceButton;
        #endregion

        #region Behaviour Callbacks
        protected override void Start() {
            m_filler.fillAmount = 0f;
            m_spaceButton.action.Enable();
        }

        private void Update() {
            if (!m_spaceButton.action.inProgress) {
                m_filler.fillAmount = Mathf.Clamp01(m_filler.fillAmount - Time.deltaTime * m_progressQuickness);
                return;
            }

            m_filler.fillAmount = Mathf.Clamp01(m_filler.fillAmount + Time.deltaTime * m_progressQuickness);

            if (m_filler.fillAmount < 1)
                return;

            GameEvents.Begin();
            Destroy(gameObject);
        }
        #endregion
    }
}