using System;
using Newtonsoft.Json;

namespace Code.Data {
    [Serializable]
    public sealed class Settings {
        [JsonProperty("video")]
        public VideoSettings Video { get; set; } = new();
        [JsonProperty("audio")]
        public AudioSettings Audio { get; set; } = new();
        [JsonProperty("game")]
        public GamePlaySettings Game { get; set; } = new();
    }
}