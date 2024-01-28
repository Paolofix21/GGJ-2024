using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.UI.Sounds
{
    public class UISoundBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
    {
        private Button button;
        private Toggle toggle;
        private Slider slider;

        private void Start()
        {
            if (GetComponent<Button>())
            {
                button = GetComponent<Button>();
                button.onClick.AddListener(ClickSound);
            }
            if (GetComponent<Toggle>())
            {
                toggle = GetComponent<Toggle>();
                toggle.onValueChanged.AddListener(delegate { ClickSound(); });
            }
            if (GetComponent<Slider>())
            {
                slider = GetComponent<Slider>();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(button)
                AudioManager.instance.PlayOneShot(FMODEvents.instance.uiClickEvent, this.transform.position);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(slider)
                AudioManager.instance.PlayOneShot(FMODEvents.instance.uiClickEvent, this.transform.position);
        }

        private void ClickSound()
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.uiClickEvent, this.transform.position);
        }

        
    }
}

