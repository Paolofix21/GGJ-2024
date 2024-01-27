using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    [SerializeField]
    private List<string> parameterValues = new List<string>{ "MAINMENU", "EXPLORATION", "BOSS" };

    private const string changerParamName = "JUMP";

    private EventInstance musicInstance;
    private EventInstance ambienceInstance;

    private void Awake()
    {
        if (instance && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);


        musicInstance = CreateInstance(FMODEvents.instance.musicEvent);
        ambienceInstance = CreateInstance(FMODEvents.instance.ambienceEvent);
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public  EventInstance CreateInstance(EventReference eventReference) 
    
    { 
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        return eventInstance;
    }

    public void PlayMainMenuMusic()
    {
        if (musicInstance.isValid())
        {
            ChangeGlobalMusicAmbienceParameter(0);
            musicInstance.start();
        }
    }

    public void PlayExplorationMusic()
    {
        if (musicInstance.isValid())
        {
            ChangeGlobalMusicAmbienceParameter(1);
            ambienceInstance.start();
        }
    }

    public void PlayBossMusic()
    {
        ChangeGlobalMusicAmbienceParameter(2);
    }


    public void ChangeGlobalMusicAmbienceParameter(int newValue)
    {
        string value = parameterValues[newValue];

        if (!string.IsNullOrEmpty(value))
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByNameWithLabel(changerParamName, value);
        }
    }
}

