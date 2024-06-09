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

            // Granted by the execution order
            WaveBasedMatchManager.Singleton.SetPlayingCharacter(this);

            Disable();
        }

        private void Start() => _controller.Health.OnPlayerDeath += OnDie;
        #endregion

        #region IPlayableCharacter
        public Transform Transform => transform;

        public void Enable() {
            if (_controller)
                _controller.enabled = true;
        }

        public void Disable() {
            _controller.enabled = false;
            //Debug.Log("Disabling...\n");
        }
        #endregion

        #region Event Methods
        private void OnDie() => OnDeath?.Invoke();
        #endregion
    }
}