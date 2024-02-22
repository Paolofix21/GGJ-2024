using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace Code.Data {
    public static class SavesManager {
        #region Public Methods
        public static bool Load(out SaveData data) {
            data = LoadFromFile();
            return data != null;
        }

        public static void Save(SaveData data) => WriteToFile(data);
        #endregion

        #region Private Methods
        private static SaveData LoadFromFile() {
            if (!File.Exists(Application.persistentDataPath + "/mecmeloffa.json"))
                return null;

            var json = File.ReadAllText(Application.persistentDataPath + "/mecmeloffa.json");
            return JsonConvert.DeserializeObject<SaveData>(json);
        }

        private static void WriteToFile(SaveData data) {
            var stringedData = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(Application.persistentDataPath + "/mecmeloffa.json", stringedData);
        }
        #endregion
    }
}