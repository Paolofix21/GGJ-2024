using System;
using Newtonsoft.Json;

namespace Code.Data {
    public enum SettingType {
        UserData,
        Audio,
        Video
    }

    [Serializable]
    public sealed class SaveData {
        [JsonProperty("high-score")]
        public int HighScore { get; set; } = 0;
        [JsonProperty("settings")]
        public Settings Settings { get; set; } = new();
    }
}