using System;
using Newtonsoft.Json;

namespace Code.Data {
    [Serializable]
    public sealed class GamePlaySettings {
        public enum Type {
            Sensitivity,
            FieldOfview
        }

        #region Properties
        [JsonProperty("send")]
        public float Sensitivity { get; set; } = 1f;
        [JsonProperty("fov")]
        public float FieldOfView { get; set; } = 60f;
        #endregion

        #region Public Methods
        public object this[Type type] {
            get => type switch {
                Type.Sensitivity => Sensitivity,
                Type.FieldOfview => FieldOfView,
                _ => null
            };
            set {
                switch (type) {
                    case Type.Sensitivity:
                        Sensitivity = (float)value;
                        break;
                    case Type.FieldOfview:
                        FieldOfView = (float)value;
                        break;
                    default:
                        return;
                }
            }
        }
        #endregion
    }
}