using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using System.Collections;
using Code.Data;
using AudioSettings = Code.Data.AudioSettings;

public class AudioManager : MonoBehaviour {
    public const string k_busGeneral = "bus:/";
    public const string k_busAmbience = "bus:/Ambience";
    public const string k_busMusic = "bus:/Music";
    public const string k_busSfx = "bus:/SFX";
    public const string k_busUi = "bus:/UI";
    public const string k_busVoiceLine = "bus:/VO";

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
        SetBusVolume(k_busGeneral, DataManager.GetVolumeSetting(AudioSettings.BusId.General));
        SetBusVolume(k_busAmbience, DataManager.GetVolumeSetting(AudioSettings.BusId.Ambience));
        SetBusVolume(k_busMusic, DataManager.GetVolumeSetting(AudioSettings.BusId.Music));
        SetBusVolume(k_busSfx, DataManager.GetVolumeSetting(AudioSettings.BusId.SoundEffect));
        SetBusVolume(k_busUi, DataManager.GetVolumeSetting(AudioSettings.BusId.UserInterface));
        SetBusVolume(k_busVoiceLine, DataManager.GetVolumeSetting(AudioSettings.BusId.VoiceLine));
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