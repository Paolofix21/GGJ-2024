using Code.Promises;
using UnityEngine;
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
        public static bool IsPausedForbidden { get; private set; }
        public static bool IsCutscenePlaying { get; private set; }

        public static bool IsOnHold => IsPaused || IsCutscenePlaying;

        public static double GameTime { get; set; }
        public static int Score { get; set; }

        public static IMatchManager MatchManager { get; set; }
        #endregion

        #region Public Methods
        public static T GetMatchManager<T>() where T : IMatchManager => (T)MatchManager;

        public static void Begin() {
            GameTime = 0;
            Score = 0;
            MatchManager?.BeginMatch();
        }

        public static void Pause() {
            if (IsPausedForbidden) {
                Debug.LogWarning("Pause is forbidden\n");
                return;
            }

            if (MatchManager == null || MatchManager.IsStopped())
                return;

            if (IsPaused)
                return;

            IsPaused = true;
            OnPauseStatusChanged?.Invoke(IsPaused);
        }

        public static void Resume() {
            Debug.Log("Resuming...\n");
            if (MatchManager == null || MatchManager.IsStopped())
                return;

            if (!IsPaused)
                return;

            IsPaused = false;
            OnPauseStatusChanged?.Invoke(IsPaused);
        }

        public static void TogglePause() {
            if (!IsPaused && IsPausedForbidden) {
                Debug.LogWarning("Pause is forbidden\n");
                return;
            }

            Debug.Log("Toggling pause\n");
            if (MatchManager == null || MatchManager.IsStopped())
                return;

            IsPaused = !IsPaused;
            OnPauseStatusChanged?.Invoke(IsPaused);
        }

        public static void ForbidPause() => IsPausedForbidden = true;
        public static void AllowPause() => IsPausedForbidden = false;

        public static void SetCutsceneState(bool isPlaying) => OnCutsceneStateChanged?.Invoke(IsCutscenePlaying = isPlaying);

        public static void Win() => OnEndGame?.Invoke(true);
        public static void Lose() => OnEndGame?.Invoke(false);

        public static void BeatHighScore(double newHighScore) => OnNewRecordBeaten?.Invoke(newHighScore);

        public static void Purge() {
            OnPauseStatusChanged = null;
            OnCutsceneStateChanged = null;
            OnEndGame = null;
            OnNewRecordBeaten = null;

            IsPaused = false;
            OnPauseStatusChanged?.Invoke(IsPaused);

            IsCutscenePlaying = false;
            OnCutsceneStateChanged?.Invoke(IsCutscenePlaying);

            MatchManager = null;
        }
        #endregion
    }
}