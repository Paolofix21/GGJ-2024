using Code.Graphics;
using System;
using Audio;
using Code.Core;
using Code.Data;
using UnityEngine;
using UnityEngine.UI;
using AudioSettings = Code.Data.AudioSettings;

namespace Code.UI {
    public class SettingsUI : MonoBehaviour {
        #region Public Variables
        [SerializeField] private VolumeSettingUI m_general;
        [SerializeField] private VolumeSettingUI m_ambience;
        [SerializeField] private VolumeSettingUI m_music;
        [SerializeField] private VolumeSettingUI m_sfx;
        [SerializeField] private VolumeSettingUI m_ui;
        [SerializeField] private VolumeSettingUI m_vo;

        [Space]
        [SerializeField] private SliderSettingUI m_sensitivity;
        [SerializeField] private SliderSettingUI m_fov;
        [SerializeField] private ToggleSettingUI m_blur;

        [Space]
        [SerializeField] private Button m_backButton;

        public static event Action<int> OnSensitivityChanged;
        public static event Action<int> OnFOVChanged;
        public static event Action<bool> OnMotionBlurChanged;
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            GameEvents.OnPauseStatusChanged += OnPauseStateChange;

            LoadValuesFromSettings();

            m_general.Register(v => OnVolumeChanged(v, AudioSettings.BusId.General));
            m_general.RegisterMute(v => OnMuteChanged(v, AudioSettings.BusId.General));

            m_ambience.Register(v => OnVolumeChanged(v, AudioSettings.BusId.Ambience));
            m_ambience.RegisterMute(v => OnMuteChanged(v, AudioSettings.BusId.Ambience));

            m_music.Register(v => OnVolumeChanged(v, AudioSettings.BusId.Music));
            m_music.RegisterMute(v => OnMuteChanged(v, AudioSettings.BusId.Music));

            m_sfx.Register(v => OnVolumeChanged(v, AudioSettings.BusId.SoundEffect));
            m_sfx.RegisterMute(v => OnMuteChanged(v, AudioSettings.BusId.SoundEffect));

            m_ui.Register(v => OnVolumeChanged(v, AudioSettings.BusId.UserInterface));
            m_ui.RegisterMute(v => OnMuteChanged(v, AudioSettings.BusId.UserInterface));

            m_vo.Register(v => OnVolumeChanged(v, AudioSettings.BusId.VoiceLine));
            m_vo.RegisterMute(v => OnMuteChanged(v, AudioSettings.BusId.VoiceLine));

            m_sensitivity.Register(ChangeSensitivity);
            m_fov.Register(ChangeFieldOfView);
            m_blur.Register(ChangeBlur);

            m_backButton.onClick.AddListener(DestroyThisGO);
        }

        private void OnDestroy() {
            GameEvents.OnPauseStatusChanged -= OnPauseStateChange;
            DataManager.Apply();
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        private void LoadValuesFromSettings() {
            m_general.SetValueSilently(DataManager.GetVolumeSetting(AudioSettings.BusId.General));
            m_ambience.SetValueSilently(DataManager.GetVolumeSetting(AudioSettings.BusId.Ambience));
            m_music.SetValueSilently(DataManager.GetVolumeSetting(AudioSettings.BusId.Music));
            m_sfx.SetValueSilently(DataManager.GetVolumeSetting(AudioSettings.BusId.SoundEffect));
            m_ui.SetValueSilently(DataManager.GetVolumeSetting(AudioSettings.BusId.UserInterface));
            m_vo.SetValueSilently(DataManager.GetVolumeSetting(AudioSettings.BusId.VoiceLine));

            m_sensitivity.SetValueSilently(DataManager.GetGamePlaySetting<int>(GamePlaySettings.Type.Sensitivity));
            m_fov.SetValueSilently(DataManager.GetGamePlaySetting<int>(GamePlaySettings.Type.FieldOfView));
            m_blur.SetValueSilently(DataManager.GetVideoSetting<bool>(VideoSettings.Type.MotionBlur));
        }

        private void DestroyThisGO() => Destroy(gameObject);
        #endregion

        #region Event Methods
        private void OnVolumeChanged(float volume, AudioSettings.BusId busId) {
            AudioManager.Singleton.SetVolume(busId, volume);
            DataManager.UpdateVolumeSetting(busId, volume);
        }

        private void OnMuteChanged(bool isMute, AudioSettings.BusId busId) => AudioManager.Singleton.SetVolume(busId, isMute ? 0f : DataManager.GetVolumeSetting(busId));

        private void ChangeSensitivity(float sens) {
            VideoSettingsHelper.MouseSensitivity = (int)sens;
            OnSensitivityChanged?.Invoke(VideoSettingsHelper.MouseSensitivity);
            DataManager.UpdateGamePlaySetting(GamePlaySettings.Type.Sensitivity, VideoSettingsHelper.MouseSensitivity);
        }

        private void ChangeFieldOfView(float fov) {
            VideoSettingsHelper.FOV = (int)fov;
            OnFOVChanged?.Invoke(VideoSettingsHelper.FOV);
            DataManager.UpdateGamePlaySetting(GamePlaySettings.Type.FieldOfView, VideoSettingsHelper.FOV);
        }

        private void ChangeBlur(bool blurOn) {
            VideoSettingsHelper.MotionBlurActive = blurOn;
            OnMotionBlurChanged?.Invoke(blurOn);
            DataManager.UpdateVideoSetting(VideoSettings.Type.MotionBlur, blurOn);
        }

        private void OnPauseStateChange(bool pause) {
            if (!pause)
                Destroy(gameObject);
        }
        #endregion
    }
}