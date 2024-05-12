using Code.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Player {
    public class PlayerInputController : MonoBehaviour {
        #region Private Variables
        [Header("Settings")]
        [SerializeField] private bool m_useInputSmoothing = false;
        [SerializeField] private float m_smoothing = 24f;

        [Header("Inputs")]
        [SerializeField] private InputActionReference m_moveInput;
        [SerializeField] private InputActionReference m_lookInput;
        [SerializeField] private InputActionReference m_pauseInput;

        [Space]
        [SerializeField] private InputActionReference m_jumpInput;
        [SerializeField] private InputActionReference m_shootInput;
        [SerializeField] private InputActionReference m_weaponCycleInput;

        [Space]
        [SerializeField] private InputActionReference m_weapon1Input;
        [SerializeField] private InputActionReference m_weapon2Input;
        [SerializeField] private InputActionReference m_weapon3Input;
        [SerializeField] private InputActionReference m_weapon4Input;
        [SerializeField] private InputActionReference m_weapon5Input;

        public event System.Action OnJump;
        public event System.Action OnShoot;
        public event System.Action<bool> OnShootStateChange;
        public event System.Action<int> OnWeaponIndexChange;
        public event System.Action<int> OnCycleWeapons;
        #endregion

        #region Private Variables
        private Vector2 _targetLookInput;
        #endregion

        #region Properties
        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            m_moveInput.action.started += OnMoveInput;
            m_moveInput.action.performed += OnMoveInput;
            m_moveInput.action.canceled += OnMoveInput;

            m_lookInput.action.started += OnLookInput;
            m_lookInput.action.performed += OnLookInput;
            m_lookInput.action.canceled += OnLookInput;

            m_pauseInput.action.canceled += OnPause;
            m_pauseInput.action.Enable();

            m_jumpInput.action.started += Jump;

            m_shootInput.action.started += Shoot;
            m_shootInput.action.canceled += Shoot;

            m_weaponCycleInput.action.started += CycleWeapons;

            m_weapon1Input.action.started += SetWeaponIndex;
            m_weapon2Input.action.started += SetWeaponIndex;
            m_weapon3Input.action.started += SetWeaponIndex;
            m_weapon4Input.action.started += SetWeaponIndex;
            m_weapon5Input.action.started += SetWeaponIndex;
        }

        private void OnEnable() {
            m_moveInput.action.Enable();
            m_lookInput.action.Enable();

            m_jumpInput.action.Enable();
            m_shootInput.action.Enable();
            m_weaponCycleInput.action.Enable();
            m_weapon1Input.action.Enable();
            m_weapon2Input.action.Enable();
            m_weapon3Input.action.Enable();
            m_weapon4Input.action.Enable();
            m_weapon5Input.action.Enable();
        }

        private void Update() {
            if (!m_useInputSmoothing)
                return;

            LookInput = Vector2.Lerp(LookInput, _targetLookInput, Time.deltaTime * m_smoothing);
        }

        private void OnDisable() {
            m_moveInput.action.Disable();
            m_lookInput.action.Disable();

            m_jumpInput.action.Disable();
            m_shootInput.action.Disable();
            m_weaponCycleInput.action.Disable();
            m_weapon1Input.action.Disable();
            m_weapon2Input.action.Disable();
            m_weapon3Input.action.Disable();
            m_weapon4Input.action.Disable();
            m_weapon5Input.action.Disable();
        }

        private void OnDestroy() {
            m_moveInput.action.started -= OnMoveInput;
            m_moveInput.action.performed -= OnMoveInput;
            m_moveInput.action.canceled -= OnMoveInput;

            m_lookInput.action.started -= OnLookInput;
            m_lookInput.action.performed -= OnLookInput;
            m_lookInput.action.canceled -= OnLookInput;

            m_pauseInput.action.canceled -= OnPause;
            m_pauseInput.action.Disable();

            m_jumpInput.action.started -= Jump;

            m_shootInput.action.started -= Shoot;
            m_shootInput.action.canceled -= Shoot;

            m_weaponCycleInput.action.started -= CycleWeapons;

            m_weapon1Input.action.started -= SetWeaponIndex;
            m_weapon2Input.action.started -= SetWeaponIndex;
            m_weapon3Input.action.started -= SetWeaponIndex;
            m_weapon4Input.action.started -= SetWeaponIndex;
            m_weapon5Input.action.started -= SetWeaponIndex;
        }
        #endregion

        #region Event Methods
        public void OnMoveInput(InputAction.CallbackContext ctx) => MoveInput = ctx.ReadValue<Vector2>();

        public void OnLookInput(InputAction.CallbackContext ctx) {
            if (m_useInputSmoothing)
                _targetLookInput = ctx.ReadValue<Vector2>();
            else
                LookInput = ctx.ReadValue<Vector2>();
        }

        public void OnPause(InputAction.CallbackContext ctx) => GameEvents.TogglePause();

        public void Jump(InputAction.CallbackContext ctx) => OnJump?.Invoke();

        public void Shoot(InputAction.CallbackContext ctx) {
            OnShoot?.Invoke();
            OnShootStateChange?.Invoke(ctx.started);
        }

        public void CycleWeapons(InputAction.CallbackContext ctx) => OnCycleWeapons?.Invoke(-(int)ctx.ReadValue<float>());

        private void SetWeaponIndex(InputAction.CallbackContext ctx) {
            if (int.TryParse(ctx.control.name, out var val))
                OnWeaponIndexChange?.Invoke(val - 1);
        }
        #endregion
    }
}