using Utilities;

namespace Code.Core {
    public static class GameEvents {
        #region Public Variables
        public static event ValueSetEventHandler<bool> OnPauseStatusChanged;
        #endregion

        #region Private Variables
        #endregion

        #region Properties
        public static bool IsPaused { get; private set; }
        #endregion

        #region Constructors
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
        #endregion

        #region Private Methods
        #endregion

        #region Event Methods
        #endregion
    }
}