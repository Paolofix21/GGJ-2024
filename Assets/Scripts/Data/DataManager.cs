using UnityEngine;

namespace Code.Data {
    public static class DataManager {
        #region Private Variables
        private static SaveData _data = new();
        #endregion

        #region Constructors
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void Init() {
            if (SavesManager.Load(out _data))
                return;

            Debug.Log("No data found to load...\n");
            _data = new SaveData();
            SavesManager.Save(_data);
        }

        public static void Apply() => SavesManager.Save(_data);
        #endregion

        #region Public Methods
        public static void UpdateHighScore(System.TimeSpan newValue) {
            if (newValue > _data.HighScore)
                _data.HighScore = newValue;
        }
        public static void GetHighScore(out System.TimeSpan highScore) => highScore = _data.HighScore;

        public static void UpdateVolumeSetting(AudioSettings.BusId busId, float volume) => _data.Settings.Audio[busId] = volume;
        public static void GetVolumeSetting(AudioSettings.BusId busId, out float volume) => volume = _data.Settings.Audio[busId];
        public static float GetVolumeSetting(AudioSettings.BusId busId) => _data.Settings.Audio[busId];

        public static void UpdateVideoSetting(VideoSettings.Type type, object value) => _data.Settings.Video[type] = value;
        public static void GetVideoSetting<T>(VideoSettings.Type type, out T volume) => volume = (T)_data.Settings.Video[type];
        public static T GetVideoSetting<T>(VideoSettings.Type type) => (T)_data.Settings.Video[type];

        public static void UpdateGamePlaySetting(GamePlaySettings.Type type, object value) => _data.Settings.Game[type] = value;
        public static void GetGamePlaySetting<T>(GamePlaySettings.Type type, out T volume) => volume = (T)_data.Settings.Game[type];
        public static T GetGamePlaySetting<T>(GamePlaySettings.Type type) => (T)_data.Settings.Game[type];
        #endregion
    }
}