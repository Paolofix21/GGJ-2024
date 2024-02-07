using Code.Promises;
using Code.Utilities;
using UnityEngine;

namespace Code.Core {
    public enum MatchState {
        Stopped = -1,
        Paused = 0,
        Ongoing = 1,
    }

    public delegate void MatchStateChangeEventHandler(MatchState state);

    [DefaultExecutionOrder(-1)]
    public abstract class MatchManager<T> : SingletonBehaviour<T> where T : MonoBehaviour {
        #region Public Variables
        public event MatchStateChangeEventHandler OnMatchStateChange;
        #endregion

        #region Private Variables
        private MatchState _state = MatchState.Stopped;
        #endregion

        #region Properties
        public MatchState State {
            get => _state;
            private set {
                if (value == _state)
                    return;

                _state = value;
                OnMatchStateChange?.Invoke(_state);
            }
        }

        public bool IsOngoing => _state != MatchState.Stopped;
        #endregion

        #region Public Methods
        public void BeginMatch() {
            if (State != MatchState.Stopped)
                return;

            OnMatchBegan();

            State = MatchState.Ongoing;
        }

        public void PauseMatch() {
            if (State != MatchState.Ongoing)
                return;

            OnMatchPaused();

            State = MatchState.Paused;
        }

        public void ResumeMatch() {
            if (State != MatchState.Paused)
                return;

            OnMatchResumed();

            State = MatchState.Ongoing;
        }

        public void EndMatch() {
            if (State == MatchState.Stopped)
                return;

            OnMatchEnded();

            State = MatchState.Stopped;
        }

        public abstract IPlayableCharacter GetPlayerEntity();
        #endregion

        #region Private Methods
        protected virtual void OnMatchBegan() {}
        protected virtual void OnMatchPaused() {}
        protected virtual void OnMatchResumed() {}
        protected virtual void OnMatchEnded() {}
        #endregion
    }
}