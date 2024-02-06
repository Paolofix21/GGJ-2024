﻿using Utilities;

namespace Code.Core {
    public static class GameEvents {
        #region Public Variables
        public static event ValueSetEventHandler<bool> OnPauseStatusChanged;
        public static event ValueSetEventHandler<bool> OnEndGame;
        #endregion

        #region Properties
        public static bool IsPaused { get; private set; }
        #endregion

        #region Public Methods
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

        public static void TogglePause(bool pause) {
            IsPaused = !IsPaused;
            OnPauseStatusChanged?.Invoke(IsPaused);
        }

        public static void Win() => OnEndGame?.Invoke(true);
        public static void Lose() => OnEndGame?.Invoke(false);
        #endregion
    }
}