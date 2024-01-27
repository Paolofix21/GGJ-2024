using UnityEngine;

namespace Code.Player
{
    public class InputManager : MonoBehaviour
    {
        public PlayerMap playerMap { get; private set; }

        private void Awake()
        {
            playerMap = new PlayerMap();
        }


        private void OnEnable()
        {
            playerMap.Enable();
        }

        private void OnDisable()
        {
            playerMap.Disable();
        }

        public Vector2 GetMovement()
        {
            return playerMap.PlayerActions.Movement.ReadValue<Vector2>();
        }

        public Vector2 CameraLookAt()
        {
            return playerMap.PlayerActions.LookAt.ReadValue<Vector2>();
        }

    }
}

