using System.Collections;
using UnityEngine;
using Code.Data;
using Code.Utilities;
using UnityEngine.Audio;
using AudioSettings = Code.Data.AudioSettings;

namespace Audio {
    public enum MixerType {
        SoundFx,
        UserInterface,
        Voice
    }

    public sealed class AudioManager : SingletonBehaviour<AudioManager> {
        #region Public Variables
        [Header("Mixers")]
        [SerializeField] private AudioMixerGroup m_generalMixer;
        [SerializeField] private AudioMixerGroup m_ambienceMixer;
        [SerializeField] private AudioMixerGroup m_musicMixer;
        [SerializeField] private AudioMixerGroup m_sfxMixer;
        [SerializeField] private AudioMixerGroup m_uiMixer;
        [SerializeField] private AudioMixerGroup m_voiceMixer;
        #endregion

        #region Private Variables
        private AudioSource _musicSource, _tempMusicSource;
        private AudioSource _ambienceSource;
        private AudioSource _uiSource;

        private Coroutine _musicCrossFadeCoroutine;
        private Coroutine _ambienceFadeCoroutine;
        private Coroutine _musicAttenuationFadeCoroutine;

        private AudioListener _tempListener;
        #endregion

        #region Behaviour Callbacks
        protected override void OnAfterAwake() {
            var musicChild = new GameObject("Music") {
                transform = {
                    parent = transform
                }
            };
            _musicSource = musicChild.AddComponent<AudioSource>();
            _musicSource.hideFlags = HideFlags.NotEditable;
            _musicSource.playOnAwake = false;
            _musicSource.loop = true;
            _musicSource.outputAudioMixerGroup = m_musicMixer;
            _musicSource.Stop();

            _ambienceSource = gameObject.AddComponent<AudioSource>();
            _ambienceSource.outputAudioMixerGroup = m_ambienceMixer;

            _uiSource = gameObject.AddComponent<AudioSource>();
            _uiSource.outputAudioMixerGroup = m_uiMixer;

            var listenerObject = new GameObject("Listener");
            _tempListener = listenerObject.AddComponent<AudioListener>();
            DontDestroyOnLoad(_tempListener);
        }

        private void Start() {
            SetMixerVolume(m_generalMixer, DataManager.GetVolumeSetting(AudioSettings.BusId.General));
            SetMixerVolume(m_ambienceMixer, DataManager.GetVolumeSetting(AudioSettings.BusId.Ambience));
            SetMixerVolume(m_musicMixer, DataManager.GetVolumeSetting(AudioSettings.BusId.Music));
            SetMixerVolume(m_sfxMixer, DataManager.GetVolumeSetting(AudioSettings.BusId.SoundEffect));
            SetMixerVolume(m_uiMixer, DataManager.GetVolumeSetting(AudioSettings.BusId.UserInterface));
            SetMixerVolume(m_voiceMixer, DataManager.GetVolumeSetting(AudioSettings.BusId.VoiceLine));
        }
        #endregion

        private void SetMixerVolume(AudioMixerGroup mixer, float value) {
            // dB = 20*log10(a)
            // a = 10^(dB/20)
            mixer.audioMixer.SetFloat($"Volume ({mixer.name})", 20 * Mathf.Log10(value));
        }

        private void SetMixerMute(AudioMixerGroup mixer, float value) {
            // dB = 20*log10(a)
            // a = 10^(dB/20)
            mixer.audioMixer.SetFloat($"Volume ({mixer.name})", 20 * Mathf.Log10(value));
        }

        public void SetVolume(AudioSettings.BusId id, float volume) {
            var mixer = id switch {
                AudioSettings.BusId.General => m_generalMixer,
                AudioSettings.BusId.Ambience => m_ambienceMixer,
                AudioSettings.BusId.Music => m_musicMixer,
                AudioSettings.BusId.SoundEffect => m_sfxMixer,
                AudioSettings.BusId.UserInterface => m_uiMixer,
                AudioSettings.BusId.VoiceLine => m_voiceMixer,
                _ => m_generalMixer
            };
            SetMixerVolume(mixer, volume);
        }

        public void MuteVolume(AudioSettings.BusId id, float volume) {
            var mixer = id switch {
                AudioSettings.BusId.General => m_generalMixer,
                AudioSettings.BusId.Ambience => m_ambienceMixer,
                AudioSettings.BusId.Music => m_musicMixer,
                AudioSettings.BusId.SoundEffect => m_sfxMixer,
                AudioSettings.BusId.UserInterface => m_uiMixer,
                AudioSettings.BusId.VoiceLine => m_voiceMixer,
                _ => m_generalMixer
            };
            SetMixerVolume(mixer, volume);
        }

        public void SetListenerState(bool active) => _tempListener.enabled = active;

        public void PlayOneShotWorld(Sound sound, Vector3 worldPos, MixerType type) {
            var go = new GameObject("play-one-shot") {
                hideFlags = HideFlags.HideInHierarchy,
                transform = {
                    position = worldPos
                }
            };

            var source = go.AddComponent<AudioSource>();
            source.clip = sound.Clip;
            source.volume = sound.Volume;
            source.spatialize = true;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.spatialBlend = 1f;
            source.outputAudioMixerGroup = type switch {
                MixerType.SoundFx => m_sfxMixer,
                MixerType.UserInterface => m_uiMixer,
                MixerType.Voice => m_voiceMixer,
                _ => m_generalMixer
            };
            source.Play();
            Destroy(go, sound.Clip.length);
        }

        public void PlayOneShotWorldAttached(Sound sound, GameObject target, MixerType type) {
            var source = target.AddComponent<AudioSource>();
            source.clip = sound.Clip;
            source.volume = sound.Volume;
            source.spatialize = true;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.spatialBlend = 1f;
            source.outputAudioMixerGroup = type switch {
                MixerType.SoundFx => m_sfxMixer,
                MixerType.UserInterface => m_uiMixer,
                MixerType.Voice => m_voiceMixer,
                _ => m_generalMixer
            };
            source.Play();
            Destroy(source, sound.Clip.length);
        }

        public void PlayOneShot2D(Sound sound, MixerType type) {
            if (type == MixerType.UserInterface) {
                PlayUiSound(sound);
                return;
            }

            var go = new GameObject("play-one-shot") {
                hideFlags = HideFlags.HideInHierarchy,
            };

            var source = go.AddComponent<AudioSource>();
            source.clip = sound.Clip;
            source.volume = sound.Volume;
            source.spatialize = false;
            source.spatialBlend = 0f;
            source.outputAudioMixerGroup = type switch {
                MixerType.SoundFx => m_sfxMixer,
                MixerType.Voice => m_voiceMixer,
                _ => m_generalMixer
            };
            source.Play();
            Destroy(go, sound.Clip.length);
        }

        public void PlayUiSound(Sound sound) => _uiSource.PlayOneShot(sound.Clip, sound.Volume);

#if UNITY_EDITOR
        public void CrossFadeMusicSimple(SoundSO sound) => CrossFadeMusic(sound.GetSound(), 4f);
#endif
        public void CrossFadeMusic(Sound sound, float fadeTime = 1f) {
            if (!_musicSource.isPlaying) {
                _musicSource.clip = sound.Clip;
                _musicSource.volume = sound.Volume;
                _musicSource.Play();
                return;
            }

            if (_musicCrossFadeCoroutine != null) {
                StopCoroutine(_musicCrossFadeCoroutine);
                EndMusicTransition();
            }

            _musicCrossFadeCoroutine = StartCoroutine(CrossFadeMusicCO(sound, fadeTime));
        }

        public void AttenuateMusic(float attenuation, float fadeDuration = 1f) {
            Debug.Log("Attenuating music...\n");
            if (_musicAttenuationFadeCoroutine != null)
                StopCoroutine(_musicAttenuationFadeCoroutine);
            _musicAttenuationFadeCoroutine = StartCoroutine(MusicAttenuationFadeCO(attenuation, fadeDuration));
        }

        public void AmbienceFadeTo(float target, float fadeTime = 1f) {
            if (_ambienceFadeCoroutine != null)
                StopCoroutine(_ambienceFadeCoroutine);

            _ambienceFadeCoroutine = StartCoroutine(AmbienceFadeToCO(target, fadeTime));
        }

        public SoundPlayer CreateSource(SoundSO sound, MixerType type, GameObject target = null) {
            if (target == null)
                target = new GameObject($"Source ({sound.name})");

            var player = target.AddComponent<SoundPlayer>();
            player.SetSound(sound);
            player.SetMixer(type switch {
                MixerType.SoundFx => m_sfxMixer,
                MixerType.Voice => m_voiceMixer,
                _ => m_generalMixer
            });

            return player;
        }

        #region Private Methods
        private IEnumerator AmbienceFadeToCO(float targetVolume, float fadeTime) {
            var startVolume = _ambienceSource.volume;
            var t = 0f;

            while (t < fadeTime) {
                t += Time.deltaTime;
                _ambienceSource.volume = Mathf.Lerp(startVolume, targetVolume, t / fadeTime);
                yield return null;
            }

            _ambienceSource.volume = targetVolume;
        }

        private IEnumerator CrossFadeMusicCO(Sound sound, float fadeTime) {
            var musicChild = new GameObject("Music (Temp)") {
                transform = {
                    parent = transform
                }
            };
            _tempMusicSource = musicChild.AddComponent<AudioSource>();
            _tempMusicSource.hideFlags = HideFlags.NotEditable;

            var startVolume = _musicSource.volume;

            _tempMusicSource.clip = sound.Clip;
            _tempMusicSource.volume = 0f;
            _tempMusicSource.outputAudioMixerGroup = m_musicMixer;
            _tempMusicSource.loop = true;
            _tempMusicSource.Play();

            var t = 0f;
            while (t < fadeTime) {
                t += Time.unscaledDeltaTime;
                var progress = t / fadeTime;
                _musicSource.volume = Mathf.Lerp(startVolume, 0f, progress);
                _tempMusicSource.volume = Mathf.Lerp(0f, sound.Volume, progress);
                yield return null;
            }

            var originalSource = _musicSource;
            _musicSource = _tempMusicSource;
            _musicSource.volume = sound.Volume;
            _musicSource.name = "Music";

            Destroy(originalSource.gameObject);
            _musicCrossFadeCoroutine = null;
        }

        private void EndMusicTransition() {
            var originalSource = _musicSource;
            _musicSource = _tempMusicSource;
            _musicSource.name = "Music";

            Destroy(originalSource.gameObject);
            _musicCrossFadeCoroutine = null;
        }

        private IEnumerator MusicAttenuationFadeCO(float attenuation, float duration = 1f) {
            var par = $"Volume ({m_musicMixer.name})";
            m_musicMixer.audioMixer.GetFloat(par, out var from);

            var musicVol = DataManager.GetVolumeSetting(AudioSettings.BusId.Music);
            var to = 20 * Mathf.Log10(musicVol * attenuation);

            var t = 0f;
            while (t < duration) {
                t += Time.unscaledDeltaTime;
                m_musicMixer.audioMixer.SetFloat(par, Mathf.Lerp(from, to, t / duration));
                yield return null;
            }

            _musicAttenuationFadeCoroutine = null;
        }
        #endregion
    }
}