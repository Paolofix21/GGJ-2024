namespace Code.Promises {
    public interface IMatchManager {
        #region Public Methods
        public void BeginMatch();
        public void PauseMatch();
        public void ResumeMatch();
        public void EndMatch();
        public IPlayableCharacter GetPlayerEntity();
        #endregion
    }
}