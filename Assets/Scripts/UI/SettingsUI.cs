using Code.Graphics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Code.UI
{
    public class SettingsUI : MonoBehaviour
    {
        #region Public Variables
        [SerializeField] private SettingsComponentUI m_general;
        [SerializeField] private SettingsComponentUI m_ambience;
        [SerializeField] private SettingsComponentUI m_music;
        [SerializeField] private SettingsComponentUI m_sfx;
        [SerializeField] private SettingsComponentUI m_ui;
        [SerializeField] private SettingsComponentUI m_vo;
        [SerializeField] private SettingsComponentUI m_sensitivity;
        [SerializeField] private SettingsComponentUI m_fov;
        [SerializeField] private SettingsComponentUI m_blur;
        [SerializeField] private Button m_backButton;
        #endregion

        #region Properties
        #endregion

        #region Behaviour Callbacks
        private void Awake()
        {
            m_general.m_Toggle.onValueChanged.RemoveAllListeners();
            m_general.m_Slider.onValueChanged.RemoveAllListeners();
            m_ambience.m_Toggle.onValueChanged.RemoveAllListeners();
            m_ambience.m_Slider.onValueChanged.RemoveAllListeners();
            m_music.m_Toggle.onValueChanged.RemoveAllListeners();
            m_music.m_Slider.onValueChanged.RemoveAllListeners();
            m_sfx.m_Toggle.onValueChanged.RemoveAllListeners();
            m_sfx.m_Slider.onValueChanged.RemoveAllListeners();
            m_ui.m_Toggle.onValueChanged.RemoveAllListeners();
            m_ui.m_Slider.onValueChanged.RemoveAllListeners();
            m_vo.m_Toggle.onValueChanged.RemoveAllListeners();
            m_vo.m_Slider.onValueChanged.RemoveAllListeners();
            m_sensitivity.m_Slider.onValueChanged.RemoveAllListeners();
            m_fov.m_Slider.onValueChanged.RemoveAllListeners();
            m_blur.m_Toggle.onValueChanged.RemoveAllListeners();
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
            Setup(m_general.m_Toggle, m_general.m_Slider, "bus:/", m_general.IsOn, m_general.m_Text);
            Setup(m_ambience.m_Toggle, m_ambience.m_Slider, "bus:/Ambience", m_ambience.IsOn, m_ambience.m_Text);
            Setup(m_music.m_Toggle, m_music.m_Slider, "bus:/Music", m_music.IsOn, m_music.m_Text);
            Setup(m_sfx.m_Toggle, m_sfx.m_Slider, "bus:/SFX", m_sfx.IsOn, m_sfx.m_Text);
            Setup(m_ui.m_Toggle, m_ui.m_Slider, "bus:/UI", m_ui.IsOn, m_ui.m_Text);
            Setup(m_vo.m_Toggle, m_vo.m_Slider, "bus:/VO", m_vo.IsOn, m_vo.m_Text);
            SettingsSetup(null, m_sensitivity.m_Slider, VideoSettingsHelper.MouseSensitivity, false, false, m_sensitivity.m_Text);
            SettingsSetup(null, m_fov.m_Slider, VideoSettingsHelper.FOV, false, false, m_fov.m_Text);
            SettingsSetup(m_blur.m_Toggle, null, 0, VideoSettingsHelper.MotionBlurActive, m_blur.IsOn, m_blur.m_Text);
            m_backButton.onClick.AddListener(DestroyThisGO);
        }
        private void Setup(Toggle myToggle, Slider mySlider, string busName, bool savedToggle, TMP_Text textValue)
        {
            FMODUnity.RuntimeManager.GetBus(busName).getVolume(out float volume);
            textValue.text = ((int)(volume * 100)).ToString();
            mySlider.value = volume;
            FMODUnity.RuntimeManager.GetBus(busName).getMute(out bool mute);
            myToggle.isOn = !mute;
            myToggle.onValueChanged.AddListener(delegate
            {
                savedToggle = ToggleVolume(!myToggle.isOn, busName);
            });
            mySlider.onValueChanged.AddListener(delegate
            {
                textValue.text = UpdateVolume(mySlider.value, busName);
            });
        }
        private void SettingsSetup(Toggle myToggle, Slider mySlider, int value, bool check, bool savedToggle, TMP_Text textValue)
        {
            if(mySlider) mySlider.value = value;
            if(myToggle) myToggle.isOn = check;

            myToggle.onValueChanged.AddListener(delegate
            {
            });
            mySlider.onValueChanged.AddListener(delegate
            {
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