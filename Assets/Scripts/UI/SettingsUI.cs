using Code.Graphics;
using System;
using System.Collections.Generic;
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
        [SerializeField] private ToggleSettingUI m_vSync;

        [Space]
        [SerializeField] private ToolbarSettingUI m_graphicsQuality;

        [Space]
        [SerializeField] private Button m_backButton;

        public static event Action<int> OnSensitivityChanged;
        public static event Action<int> OnFOVChanged;
        public static event Action<bool> OnMotionBlurChanged;
        public static event Action<bool> OnVSyncChanged;
        public static event Action<int> OnGraphicsQualityChanged;
        #endregion

        #region Behaviour Callbacks
        private void Start() {
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
            m_vSync.Register(ChangeVSync);

            m_graphicsQuality.Register(ChangeGraphicsQuality);

            m_backButton.onClick.AddListener(DestroyThisGo);
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
            DataManager.GetVolumeSetting(AudioSettings.BusId.General, out var volSettingGeneral);
            DataManager.GetVolumeSetting(AudioSettings.BusId.Ambience, out var volSettingAmbience);
            DataManager.GetVolumeSetting(AudioSettings.BusId.Music, out var volSettingMusic);
            DataManager.GetVolumeSetting(AudioSettings.BusId.SoundEffect, out var volSettingSfx);
            DataManager.GetVolumeSetting(AudioSettings.BusId.UserInterface, out var volSettingUi);
            DataManager.GetVolumeSetting(AudioSettings.BusId.VoiceLine, out var volSettingVoice);

            m_general.SetValueSilently(volSettingGeneral.Volume);
            m_ambience.SetValueSilently(volSettingAmbience.Volume);
            m_music.SetValueSilently(volSettingMusic.Volume);
            m_sfx.SetValueSilently(volSettingSfx.Volume);
            m_ui.SetValueSilently(volSettingUi.Volume);
            m_vo.SetValueSilently(volSettingVoice.Volume);

            m_general.SetMuteSilently(volSettingGeneral.IsMute);
            m_ambience.SetMuteSilently(volSettingAmbience.IsMute);
            m_music.SetMuteSilently(volSettingMusic.IsMute);
            m_sfx.SetMuteSilently(volSettingSfx.IsMute);
            m_ui.SetMuteSilently(volSettingUi.IsMute);
            m_vo.SetMuteSilently(volSettingVoice.IsMute);

            m_sensitivity.SetValueSilently(DataManager.GetGamePlaySetting<int>(GamePlaySettings.Type.Sensitivity));
            m_fov.SetValueSilently(DataManager.GetGamePlaySetting<int>(GamePlaySettings.Type.FieldOfView));
            m_blur.SetValueSilently(DataManager.GetVideoSetting<bool>(VideoSettings.Type.MotionBlur));
            m_vSync.SetValueSilently(DataManager.GetVideoSetting<bool>(VideoSettings.Type.VSync));

            m_graphicsQuality.SetValueSilently(DataManager.GetVideoSetting<int>(VideoSettings.Type.VideoQuality));
        }

        private void DestroyThisGo() => Destroy(gameObject);
        #endregion

        #region Event Methods
        private void OnVolumeChanged(float volume, AudioSettings.BusId busId) {
            if (!DataManager.GetVolumeSetting(busId).IsMute)
                AudioManager.Singleton.SetVolume(busId, volume);
            DataManager.UpdateVolumeSetting(busId, volume);
        }

        private void OnMuteChanged(bool isMute, AudioSettings.BusId busId) {
            AudioManager.Singleton.SetMute(busId, isMute);
            DataManager.UpdateVolumeSetting(busId, isMute);
        }

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

        private void ChangeVSync(bool vSyncOn) {
            VideoSettingsHelper.VSync = vSyncOn;
            OnVSyncChanged?.Invoke(vSyncOn);
            DataManager.UpdateVideoSetting(VideoSettings.Type.VSync, vSyncOn);
        }

        private void ChangeGraphicsQuality(int graphicsQualityIndex) {
            VideoSettingsHelper.VideoQuality = graphicsQualityIndex;
            OnGraphicsQualityChanged?.Invoke(graphicsQualityIndex);
            DataManager.UpdateVideoSetting(VideoSettings.Type.VideoQuality, graphicsQualityIndex);
        }

        private void OnPauseStateChange(bool pause) {
            if (!pause)
                Destroy(gameObject);
        }
        #endregion
    }
}