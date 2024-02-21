using System;
using Newtonsoft.Json;

namespace Code.Data {
    [Serializable]
    public sealed class GamePlaySettings {
        public enum Type {
            Sensitivity,
            FieldOfView
        }

        #region Properties
        [JsonProperty("send")]
        public int Sensitivity { get; set; } = 10;
        [JsonProperty("fov")]
        public int FieldOfView { get; set; } = 60;
        #endregion

        #region Public Methods
        public object this[Type type] {
            get => type switch {
                Type.Sensitivity => Sensitivity,
                Type.FieldOfView => FieldOfView,
                _ => null
            };
            set {
                switch (type) {
                    case Type.Sensitivity:
                        Sensitivity = (int)value;
                        break;
                    case Type.FieldOfView:
                        FieldOfView = (int)value;
                        break;
                    default:
                        return;
                }
            }
        }
        #endregion
    }
}