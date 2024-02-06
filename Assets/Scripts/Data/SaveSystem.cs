using System;
using UnityEngine;
using Newtonsoft.Json;
namespace Code.Data
{
    public class SaveSystem : MonoBehaviour
    {
        public static SaveData dataStore = new();
        private void Awake()
        {
            dataStore = GetData();
            if(dataStore == null )
            {
                Debug.Log("No data found.");
            }
        }
        public SaveData GetData()
        {
            if (!System.IO.File.Exists(Application.persistentDataPath + "/mecmeloffa.json"))
                return null;
            string data = System.IO.File.ReadAllText(Application.persistentDataPath + "/mecmeloffa.json");
            return JsonConvert.DeserializeObject<SaveData>(data);
        }
        public static void SaveData(SaveData newData)
        {
            dataStore = newData;
            string stringedData = JsonConvert.SerializeObject(newData);
            System.IO.File.WriteAllText(Application.persistentDataPath + "/mecmeloffa.json", stringedData);
        }
    }
    [Serializable]
    public class SaveData
    {
        [SerializeField] public static Data data;
    }
    [Serializable]
    public class Data
    {
        [JsonProperty("highscore")] public string Highscore { get; set; }
        [JsonProperty("settings")] public Settings Settings { get; set; }
    }
    [Serializable]
    public class Settings
    {
        [JsonProperty("video")] public Video Video { get; set; }
        [JsonProperty("audio")] public Audio Audio { get; set; }
    }
    [Serializable]
    public class Audio
    {
        [JsonProperty("general")] public string General { get; set; }
        [JsonProperty("music")] public string Music { get; set; }
        [JsonProperty("ambience")] public string Ambience { get; set; }
        [JsonProperty("soundeffect")] public string SoundEffect { get; set; }
        [JsonProperty("ui")] public string UI { get; set; }
        [JsonProperty("vo")] public string VO { get; set; }
    }
    [Serializable]
    public class Video
    {
        [JsonProperty("blur")] public bool MotionBlur;
    }
}
