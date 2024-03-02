using Code.Core;

namespace Code.Promises {
    public interface IMatchManager {
        #region Public Methods
        public MatchState State { get; }

        public void BeginMatch();
        public void PauseMatch();
        public void ResumeMatch();
        public void EndMatch();
        public IPlayableCharacter GetPlayerEntity();

        public bool IsStopped() => State == MatchState.Stopped;
        #endregion
    }
}