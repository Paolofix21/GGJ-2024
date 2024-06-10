using System;
using Newtonsoft.Json;

namespace Code.Data {
    [Serializable]
    public sealed class VideoSettings {
        public enum Type {
            MotionBlur,
            VSync,
            VideoQuality
        }

        #region Properties
        [JsonProperty("blur")]
        public bool MotionBlur { get; set; } = true;
        [JsonProperty("v-sync")]
        public bool VSync { get; set; } = false;
        [JsonProperty("quality")]
        public int Quality { get; set; } = 2;
        #endregion

        #region Public Methods
        public object this[Type type] {
            get => type switch {
                Type.MotionBlur => MotionBlur,
                Type.VSync => VSync,
                Type.VideoQuality => Quality,
                _ => null
            };
            set {
                switch (type) {
                    case Type.MotionBlur:
                        MotionBlur = (bool)value;
                        break;
                    case Type.VSync:
                        VSync = (bool)value;
                        break;
                    case Type.VideoQuality:
                        Quality = (int)value;
                        break;
                    default:
                        return;
                }
            }
        }
        #endregion
    }
}