using Code.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Player {
    public class InputManager : MonoBehaviour {
        #region Properties
        public PlayerMap playerMap { get; private set; }
        #endregion

        #region Behaviour Callbacks
        private void Awake() => playerMap = new PlayerMap();

        private void Start() => playerMap.HUD.Menu.started += PauseMenu;

        private void OnDestroy() => playerMap.HUD.Menu.started -= PauseMenu;

        private void OnEnable() => playerMap.Enable();

        private void OnDisable() => playerMap.Disable();
        #endregion

        #region Event Methods
        public Vector2 GetMovement() => playerMap.PlayerActions.Movement.ReadValue<Vector2>();

        public Vector2 CameraLookAt() => playerMap.PlayerActions.LookAt.ReadValue<Vector2>();

        public void PauseMenu(InputAction.CallbackContext context) {
            context.ReadValueAsButton();
            GameEvents.TogglePause();
        }
        #endregion
    }
}