using UnityEngine;
using UnityEngine.Audio;

namespace Audio {
    public enum StopMode {
        Sudden,
        Fade
    }

    [RequireComponent(typeof(AudioSource))]
    public sealed class SoundPlayer : MonoBehaviour {
        #region Public Variables
        [field: SerializeField] public SoundSO Sound { get; private set; }
        #endregion

        #region Private Variables
        private AudioSource _source;
        #endregion

        #region Properties
        public bool IsPlaying => _source.isPlaying;
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            if (!TryGetComponent(out _source))
                _source = gameObject.AddComponent<AudioSource>();
        }
        #endregion

        #region Public Methods
        public void Play() {
            if (IsPlaying)
                return;

            if (Sound is SoundGroupSO)
                PlaySoundGroup();
            else {
                var sound = Sound.GetSound();
                _source.clip = sound.Clip;
                _source.volume = sound.Volume;
                _source.Play();
            }
        }

        public void Stop(StopMode mode) {
            if (!IsPlaying)
                return;

            if (Sound is SoundGroupSO)
                CancelInvoke(nameof(PlaySoundGroup));
            else
                _source.Stop();
        }

        public void SetSound(SoundSO sound) => Sound = sound;

        public void SetMixer(AudioMixerGroup mixer) => _source.outputAudioMixerGroup = mixer;
        #endregion

        #region Private Methods
        private void PlaySoundGroup() {
            var sound = Sound.GetSound();
            _source.PlayOneShot(sound.Clip, sound.Volume);
            Invoke(nameof(PlaySoundGroup), sound.Clip.length);
        }
        #endregion

        #region Event Methods
        #endregion
    }
}