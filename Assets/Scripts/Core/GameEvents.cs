using Code.Promises;
using Utilities;

namespace Code.Core {
    public static class GameEvents {
        #region Public Variables
        public static event ValueSetEventHandler<bool> OnPauseStatusChanged;
        public static event ValueSetEventHandler<bool> OnCutsceneStateChanged;
        public static event ValueSetEventHandler<bool> OnEndGame;
        public static event ValueSetEventHandler<double> OnNewRecordBeaten;
        #endregion

        #region Properties
        public static bool IsPaused { get; private set; }
        public static bool IsCutscenePlaying { get; private set; }

        public static bool IsOnHold => IsPaused || IsCutscenePlaying;

        public static IMatchManager MatchManager { get; set; }
        #endregion

        #region Public Methods
        public static T GetMatchManager<T>() where T : IMatchManager => (T)MatchManager;

        public static void Pause() {
            if (IsPaused)
                return;

            IsPaused = true;
            OnPauseStatusChanged?.Invoke(IsPaused);
        }

        public static void Resume() {
            if (!IsPaused)
                return;

            IsPaused = false;
            OnPauseStatusChanged?.Invoke(IsPaused);
        }

        public static void TogglePause() {
            IsPaused = !IsPaused;
            OnPauseStatusChanged?.Invoke(IsPaused);
        }

        public static void SetCutsceneState(bool isPlaying) => OnCutsceneStateChanged?.Invoke(IsCutscenePlaying = isPlaying);

        public static void Win() => OnEndGame?.Invoke(true);
        public static void Lose() => OnEndGame?.Invoke(false);

        public static void BeatHighScore(double newHighScore) => OnNewRecordBeaten?.Invoke(newHighScore);
        #endregion
    }
}