using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Code.UI
{
    public class SettingsUI : MonoBehaviour
    {
        #region Public Variables
        [SerializeField] private Toggle m_musicToggle;
        [SerializeField] private Toggle m_sfxToggle;
        [SerializeField] private Slider m_musicSlider;
        [SerializeField] private Slider m_sfxSlider;
        [SerializeField] private TMP_Text m_musicValueToDisplay;
        [SerializeField] private TMP_Text m_sfxValueToDisplay;
        [SerializeField] private Button m_backButton;
        #endregion

        #region Properties
        #endregion

        #region Private Variables
        private bool _musicOn;
        private bool _sfxOn;
        #endregion

        #region Behaviour Callbacks
        private void Awake()
        {
            m_musicToggle.onValueChanged.RemoveAllListeners();
            m_sfxToggle.onValueChanged.RemoveAllListeners();
            m_musicSlider.onValueChanged.RemoveAllListeners();
            m_sfxSlider.onValueChanged.RemoveAllListeners();
            m_backButton.onClick.RemoveAllListeners();
        }
        private void Start()
        {
            SetupEverything();
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        private void SetupEverything()
        {
            Setup(m_musicToggle, m_musicSlider, "bus:/Music", _musicOn, m_musicValueToDisplay);
            Setup(m_sfxToggle, m_sfxSlider, "bus:/SFX", _sfxOn, m_sfxValueToDisplay);
            m_backButton.onClick.AddListener(DestroyThisGO);
        }
        private void Setup(Toggle myToggle, Slider mySlider, string busName, bool savedToggle, TMP_Text textValue)
        {
            FMODUnity.RuntimeManager.GetBus(busName).getVolume(out float volume);
            textValue.text = ((int)(volume * 100)).ToString();
            mySlider.value = volume;
            myToggle.isOn = !FMODUnity.RuntimeManager.IsMuted;
            myToggle.onValueChanged.AddListener(delegate
            {
                savedToggle = ToggleVolume(!myToggle.isOn, busName);
            });
            mySlider.onValueChanged.AddListener(delegate
            {
                textValue.text = UpdateVolume(mySlider.value, busName);
            });
        }
        private bool ToggleVolume(bool isOn, string busName)
        {
            FMOD.Studio.Bus bus = FMODUnity.RuntimeManager.GetBus(busName);
            bus.setMute(isOn);
            return isOn;
        }
        private string UpdateVolume(float value, string busName)
        {
            FMOD.Studio.Bus bus = FMODUnity.RuntimeManager.GetBus(busName);
            bus.setVolume(value);
            int conversion = (int)(value * 100);
            string valueToDisplay = conversion.ToString();
            return valueToDisplay;
        }
        private void DestroyThisGO()
        {
            Destroy(this.gameObject);
        }
        #endregion

        #region Virtual Methods
        #endregion
    }
}