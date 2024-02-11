using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using System.Collections;
using Code.Data;
using AudioSettings = Code.Data.AudioSettings;

public class AudioManager : MonoBehaviour {
    public static AudioManager instance { get; private set; }

    [SerializeField]
    private List<string> parameterValues = new List<string> { "MAINMENU", "EXPLORATION", "BOSS" };

    private const string changerParamName = "JUMP";

    private EventInstance musicInstance;
    private EventInstance ambienceInstance;

    #region Behaviour Callbacks
    private void Awake() {
        if (instance && instance != this) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        musicInstance = CreateInstance(FMODEvents.instance.musicEvent);
        ambienceInstance = CreateInstance(FMODEvents.instance.ambienceEvent);
    }

    private void Start() {
        SetBusVolume("bus:/", DataManager.GetVolumeSetting(AudioSettings.BusId.General));
        SetBusVolume("bus:/Ambience", DataManager.GetVolumeSetting(AudioSettings.BusId.Ambience));
        SetBusVolume("bus:/Music", DataManager.GetVolumeSetting(AudioSettings.BusId.Music));
        SetBusVolume("bus:/SFX", DataManager.GetVolumeSetting(AudioSettings.BusId.SoundEffect));
        SetBusVolume("bus:/UI", DataManager.GetVolumeSetting(AudioSettings.BusId.UserInterface));
        SetBusVolume("bus:/VO", DataManager.GetVolumeSetting(AudioSettings.BusId.VoiceLine));
    }

    private void OnDestroy() {
        if (instance && instance == this)
            instance = null;
    }
    #endregion

    private void SetBusVolume(string busName, float value) {
        var bus = RuntimeManager.GetBus(busName);
        bus.setVolume(value);
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos = default) => RuntimeManager.PlayOneShot(sound, worldPos);

    public EventInstance CreateInstance(EventReference eventReference) {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        return eventInstance;
    }

    public void PlayMainMenuMusic() {
        if (musicInstance.isValid()) {
            ChangeGlobalMusicAmbienceParameter(0);
            musicInstance.start();
        }
    }

    public void PlayExplorationMusic() {
        if (musicInstance.isValid()) {
            ChangeGlobalMusicAmbienceParameter(1);
            ambienceInstance.start();
        }
    }

    public void PlayBossMusic() => ChangeGlobalMusicAmbienceParameter(2);

    public void ChangeGlobalMusicAmbienceParameter(int newValue) {
        string value = parameterValues[newValue];

        if (!string.IsNullOrEmpty(value)) {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByNameWithLabel(changerParamName, value);
        }
    }
}