using Code.Promises;
using UnityEngine;
using Utilities;

namespace Code.Core.MatchManagers {
    public sealed class WaveBasedMatchManager : MatchManager<WaveBasedMatchManager> {
        #region Public Variables
        public event ValueSetEventHandler<IPlayableCharacter> OnPlayingCharacterChanged; 
        public event ValueSetEventHandler<IEntity> OnBossChanged; 
        public event ValueSetEventHandler<IEntityManager> OnEntityManagerChanged; 
        #endregion

        #region Private Variables
        #endregion

        #region Properties
        public IPlayableCharacter Character { get; private set; }
        public IEntity Boss { get; private set; }
        public IEntityManager EntityManager { get; private set; }

        public TimeCounter Timer { get; } = new();
        #endregion

        #region Behaviour Callbacks
        protected override void OnAfterAwake() {
            GameEvents.OnPauseStatusChanged += TogglePause;
        }

        protected override void OnBeforeDestroy() {
            GameEvents.OnPauseStatusChanged -= TogglePause;
        }
        #endregion

        #region Overrides
        protected override void OnMatchBegan() {
            if (Character == null) {
                Debug.LogError("Character instance is not set on the Match Manager\n", this);
                return;
            }

            Timer.Start();

            EntityManager.Enable();
            Character.Enable();
        }

        protected override void OnMatchPaused() {
            Timer.Pause();

            EntityManager.Disable();
            Character.Disable();
        }

        protected override void OnMatchResumed() {
            Character.Enable();
            EntityManager.Enable();

            Timer.Resume();
        }

        protected override void OnMatchEnded() {
            Character.Disable();
            EntityManager.Disable();

            Timer.End();
        }
        #endregion

        #region Public Methods
        public void SetPlayingCharacter(IPlayableCharacter character) {
            Character = character;
            OnPlayingCharacterChanged?.Invoke(Character);
        }

        public void SetBoss(IEntity boss) {
            Boss = boss;
            OnBossChanged?.Invoke(Boss);
        }

        public void SetEntityManager(IEntityManager manager) {
            EntityManager = manager;
            OnEntityManagerChanged?.Invoke(EntityManager);
        }
        #endregion

        #region Private Methods
        #endregion

        #region Event Methods
        private void TogglePause(bool pause) {
            if (pause)
                PauseMatch();
            else
                ResumeMatch();
        }
        #endregion
    }
}