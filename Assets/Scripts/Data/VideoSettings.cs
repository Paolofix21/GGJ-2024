using System;
using Newtonsoft.Json;

namespace Code.Data {
    [Serializable]
    public sealed class VideoSettings {
        public enum Type {
            MotionBlur
        }

        #region Properties
        [JsonProperty("blur")]
        public bool MotionBlur { get; set; } = true;
        #endregion

        #region Public Methods
        public object this[Type type] {
            get => type switch {
                Type.MotionBlur => MotionBlur,
                _ => null
            };
            set {
                switch (type) {
                    case Type.MotionBlur:
                        MotionBlur = (bool)value;
                        break;
                    default:
                        return;
                }
            }
        }
        #endregion
    }
}