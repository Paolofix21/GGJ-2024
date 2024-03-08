using Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.UI.Sounds {
    public class UISoundBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler {
        [SerializeField] private SoundSO m_uiHoverEvent;
        [SerializeField] private SoundSO m_uiClickEvent;

        private Button button;
        private Toggle toggle;
        private Slider slider;

        private void Start() {
            if (GetComponent<Button>()) {
                button = GetComponent<Button>();
                button.onClick.AddListener(ClickSound);
            }

            if (GetComponent<Toggle>()) {
                toggle = GetComponent<Toggle>();
                toggle.onValueChanged.AddListener(delegate { ClickSound(); });
            }

            if (GetComponent<Slider>())
                slider = GetComponent<Slider>();
        }

        public void OnPointerEnter(PointerEventData eventData) => AudioManager.Singleton.PlayUiSound(m_uiHoverEvent.GetSound());

        public void OnPointerDown(PointerEventData eventData) {
            if (slider)
                AudioManager.Singleton.PlayUiSound(m_uiClickEvent.GetSound());
        }

        private void ClickSound() => AudioManager.Singleton.PlayUiSound(m_uiClickEvent.GetSound());
    }
}