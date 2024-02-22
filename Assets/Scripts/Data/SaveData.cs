using System;
using Newtonsoft.Json;

namespace Code.Data {
    public enum SettingType {
        UserData,
        Audio,
        Video,
        GamePlay
    }

    [Serializable]
    public sealed class SaveData {
        [JsonProperty("high-score")]
        public double HighScore { get; set; } = double.MaxValue;
        [JsonProperty("settings")]
        public Settings Settings { get; set; } = new();
    }
}