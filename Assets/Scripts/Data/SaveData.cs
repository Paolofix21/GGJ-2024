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
        public TimeSpan HighScore { get; set; } = TimeSpan.MaxValue;
        [JsonProperty("settings")]
        public Settings Settings { get; set; } = new();
    }
}