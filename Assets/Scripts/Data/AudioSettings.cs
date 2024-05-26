using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Code.Data {
    [Serializable]
    public sealed class AudioChannelSetting {
        [JsonProperty("volume")]
        public float Volume { get; set; }
        [JsonProperty("mute")]
        public bool IsMute { get; set; }

        public float GetVolume() => IsMute ? 0f : Volume;

        public static implicit operator AudioChannelSetting(float volume) => new() {
            Volume = volume,
            IsMute = false
        };
    }

    [Serializable]
    public sealed class AudioSettings {
        public enum BusId {
            General,
            Music,
            Ambience,
            SoundEffect,
            UserInterface,
            VoiceLine
        }

        #region Properties
        [JsonProperty("general")]
        public AudioChannelSetting General { get; set; } = .5f;
        [JsonProperty("music")]
        public AudioChannelSetting Music { get; set; } = 1f;
        [JsonProperty("ambience")]
        public AudioChannelSetting Ambience { get; set; } = 1f;
        [JsonProperty("sound-effect")]
        public AudioChannelSetting SoundEffect { get; set; } = .5f;
        [JsonProperty("ui")]
        public AudioChannelSetting UserInterface { get; set; } = 1f;
        [JsonProperty("vo")]
        public AudioChannelSetting VoiceLine { get; set; } = 1f;
        #endregion

        #region Public Methods
        public AudioChannelSetting this[BusId id] {
            get => id switch {
                BusId.General => General,
                BusId.Music => Music,
                BusId.Ambience => Ambience,
                BusId.SoundEffect => SoundEffect,
                BusId.UserInterface => UserInterface,
                BusId.VoiceLine => VoiceLine,
                _ => 0f
            };
            set {
                switch (id) {
                    case BusId.General:
                        General = value;
                        break;
                    case BusId.Music:
                        Music = value;
                        break;
                    case BusId.Ambience:
                        Ambience = value;
                        break;
                    case BusId.SoundEffect:
                        SoundEffect = value;
                        break;
                    case BusId.UserInterface:
                        UserInterface = value;
                        break;
                    case BusId.VoiceLine:
                        VoiceLine = value;
                        break;
                    default: return;
                }
            }
        }
        #endregion
    }
}