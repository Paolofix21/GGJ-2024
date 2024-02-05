using Code.Core.MatchManagers;
using Code.Player;
using Code.Promises;
using UnityEngine;

namespace Code.GameModeUtils.WaveBasedMode {
    [RequireComponent(typeof(PlayerController))]
    public sealed class WaveBasedPlayerEntity : MonoBehaviour, IPlayableCharacter {
        #region Public Variables
        public event System.Action OnDeath;
        #endregion

        #region Private Variables
        private PlayerController _controller;
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            _controller = GetComponent<PlayerController>();
            _controller.Health.OnPlayerDeath += OnDie;

            Disable();
        }

        private void Start() {
            WaveBasedMatchManager.Singleton.SetPlayingCharacter(this);
        }
        #endregion

        #region IPlayableCharacter
        public void Enable() {
            _controller.enabled = true;
        }

        public void Disable() {
            _controller.enabled = false;
        }
        #endregion

        #region Event Methods
        private void OnDie() => OnDeath?.Invoke();
        #endregion
    }
}