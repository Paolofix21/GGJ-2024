using System;
using Newtonsoft.Json;

namespace Code.Data {
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
        public float General { get; set; } = .5f;
        [JsonProperty("music")]
        public float Music { get; set; } = 1f;
        [JsonProperty("ambience")]
        public float Ambience { get; set; } = 1f;
        [JsonProperty("sound-effect")]
        public float SoundEffect { get; set; } = .5f;
        [JsonProperty("ui")]
        public float UserInterface { get; set; } = 1f;
        [JsonProperty("vo")]
        public float VoiceLine { get; set; } = 1f;
        #endregion

        #region Public Methods
        public float this[BusId id] {
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