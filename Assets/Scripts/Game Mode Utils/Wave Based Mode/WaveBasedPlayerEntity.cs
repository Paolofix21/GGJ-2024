using Code.Core.MatchManagers;
using Code.Player;
using Code.Promises;
using UnityEngine;

namespace Code.GameModeUtils.WaveBasedMode {
    [RequireComponent(typeof(PlayerController))]
    public sealed class WaveBasedPlayerEntity : MonoBehaviour, IPlayableCharacter {
        #region Public Variables
        #endregion

        #region Private Variables
        private PlayerController _controller;
        #endregion

        #region Properties
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            _controller = GetComponent<PlayerController>();

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

        #region Public Methods
        #endregion

        #region Private Methods
        #endregion

        #region Event Methods
        #endregion
    }
}