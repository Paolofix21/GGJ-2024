using Code.GameModeUtils.WaveBasedMode;
using UnityEngine;
using Utilities;

namespace Code.Core.MatchManagers {
    [DefaultExecutionOrder(-2)]
    public sealed class WaveBasedMatchManager : MatchManager<WaveBasedMatchManager> {
        #region Public Variables
        [SerializeField] private bool m_beginOnStart = false;

        public event ValueSetEventHandler<WaveBasedPlayerEntity> OnPlayingCharacterChanged; 
        public event ValueSetEventHandler<WaveBasedBossEntity> OnBossChanged; 
        public event ValueSetEventHandler<WaveBasedEntityManager> OnEntityManagerChanged; 
        #endregion

        #region Properties
        public WaveBasedPlayerEntity Character { get; private set; }
        public WaveBasedBossEntity Boss { get; private set; }
        public WaveBasedEntityManager EntityManager { get; private set; }

        public TimeCounter Timer { get; } = new();
        #endregion

        #region Behaviour Callbacks
        protected override void OnAfterAwake() {
            GameEvents.OnPauseStatusChanged += TogglePause;
        }

        private System.Collections.IEnumerator Start() {
            yield return null;

            if (m_beginOnStart)
                BeginMatch();
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

            EntityManager.Begin();
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
            EntityManager.End();

            Timer.End();
        }
        #endregion

        #region Public Methods
        public void SetPlayingCharacter(WaveBasedPlayerEntity character) {
            if (Character)
                Character.OnDeath -= OnLose;

            Character = character;
            Character.OnDeath += OnLose;
            OnPlayingCharacterChanged?.Invoke(Character);
        }

        public void SetBoss(WaveBasedBossEntity boss) {
            if (Boss)
                Boss.OnFinish -= OnWin;

            Boss = boss;
            Boss.OnFinish += OnWin;
            OnBossChanged?.Invoke(Boss);
        }

        public void SetEntityManager(WaveBasedEntityManager manager) {
            if (EntityManager)
                EntityManager.OnFinish -= OnWavesOver;

            EntityManager = manager;
            EntityManager.OnFinish += OnWavesOver;
            OnEntityManagerChanged?.Invoke(EntityManager);
        }
        #endregion

        #region Event Methods
        private void TogglePause(bool pause) {
            if (pause)
                PauseMatch();
            else
                ResumeMatch();
        }

        private void OnWavesOver() {
            Boss.Enable();
            Boss.StartFight();
            Character.SendMessage("BoostAllWeapons");
        }

        private void OnWin() {
            GameEvents.Win();
            EndMatch();
        }

        private void OnLose() {
            GameEvents.Lose();
            EndMatch();
        }
        #endregion
    }
}