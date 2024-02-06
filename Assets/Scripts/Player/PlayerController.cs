using Code.Graphics;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using FMOD.Studio;
using Barbaragno.RuntimePackages.Operations;
using FMODUnity;
using Izinspector.Runtime.PropertyAttributes;
using Miscellaneous;

namespace Code.Player {
    [DefaultExecutionOrder(1)]
    public class PlayerController : MonoBehaviour {
        #region Public Variables
        [Header("References")]
        [SerializeField] private ColorSetSO[] hueValue;
        [SerializeField] private GameObject arms;
        [SerializeField] private Animator anim;

        [Header("Movement Fields")]
        [SerializeField] private float airborneSpeed = 16f;
        [SerializeField] private float lavaSpeed = 8f;
        [SerializeField] private float speed = 6f;
        [Space(2)]
        [SerializeField] private float jumpForce = 1.5f;

        [Header("Lava Fields")]
        [SerializeField] private float lavaJumpForce = 1.5f;
        [SerializeField] private int timeDelay;
        [SerializeField] private int lavaDamage;
        [Space(2)]
        [SerializeField] private float jumpCooldown = 1.5f;

        [Space]
        [SerializeField, Tag] private string lavaLayer;

        public event Action<int> OnWeaponChanged;
        public event Func<bool> OnShootRequest;
        #endregion

        #region Properties
        public static PlayerController Singleton { get; set; }

        public PlayerHealth Health { get; private set; }
        public VisualSetter VisualSetter { get; private set; }
        #endregion

        #region Private Variables
        private CharacterController controller;
        private PlayerView cameraLook;
        private InputManager _input;

        private EventInstance _footstepsInstance;

        private bool _isDead;
        private bool _isInsideLava;

        private Vector3 _vel;
        private float _currentSpeed;

        private int _currentSelectedWeapon;
        private float _currentCooldownValue;
        #endregion

        #region Animations
        private static readonly int AnimProp_IsShooting = Animator.StringToHash("Is Shooting");
        private static readonly int AnimProp_ShootTrigger = Animator.StringToHash("Shoot");
        private static readonly int AnimProp_WeaponType = Animator.StringToHash("Weapon Type Index");

        private static readonly int AnimState_Pistol = Animator.StringToHash("Pistol Unholster");
        private static readonly int AnimState_Rifle = Animator.StringToHash("Rifle Unholster");
        private static readonly int AnimState_Shotgun = Animator.StringToHash("Shotgun Unholster");
        private static readonly int AnimState_Frustino = Animator.StringToHash("Frustino Unholster");
        private static readonly int AnimState_Sword = Animator.StringToHash("Sword Unholster");
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake() {
            if (Singleton && Singleton != this) {
                Destroy(gameObject);
                return;
            }

            Singleton = this;

            controller = GetComponent<CharacterController>();
            cameraLook = GetComponent<PlayerView>();
            Health = GetComponent<PlayerHealth>();
            VisualSetter = GetComponentInChildren<VisualSetter>();
            _input = GetComponent<InputManager>();

            _currentSpeed = speed;
        }

        private void OnEnable() {
            _input.playerMap.PlayerActions.Jump.started += Jump;
            _input.playerMap.PlayerActions.Shoot.started += PlayShoot;

            _input.playerMap.PlayerActions.ContinuousShoot.started += ShootContinuousInput;
            _input.playerMap.PlayerActions.ContinuousShoot.canceled += ShootContinuousInput;

            _input.playerMap.PlayerActions.Weapon01.started += SetWeaponInput;
            _input.playerMap.PlayerActions.Weapon02.started += SetWeaponInput;
            _input.playerMap.PlayerActions.Weapon03.started += SetWeaponInput;
            _input.playerMap.PlayerActions.Weapon04.started += SetWeaponInput;
            _input.playerMap.PlayerActions.Weapon05.started += SetWeaponInput;

            _input.playerMap.PlayerActions.RotateWeapon.started += TestRotateWeapons;

            cameraLook.enabled = true;
        }

        private void Start() {
            CutsceneIntroController.OnIntroStartStop += Intro;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Health.OnPlayerDeath += PlayerDeath;

            InitializeAudio();
        }

        private void Update() {
            if (_isDead)
                return;

            GetMovement();
            UpdateSound();
            cameraLook.GetMousePos(_input.CameraLookAt());
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag(lavaLayer))
                StartCoroutine(WalkInLava());
        }

        private void OnTriggerExit(Collider other) {
            if (other.gameObject.CompareTag(lavaLayer))
                ExitLava();
        }

        private void OnDisable() {
            _input.playerMap.PlayerActions.Jump.started -= Jump;
            _input.playerMap.PlayerActions.Shoot.started -= PlayShoot;

            _input.playerMap.PlayerActions.ContinuousShoot.started -= ShootContinuousInput;
            _input.playerMap.PlayerActions.ContinuousShoot.canceled -= ShootContinuousInput;

            _input.playerMap.PlayerActions.Weapon01.started -= SetWeaponInput;
            _input.playerMap.PlayerActions.Weapon02.started -= SetWeaponInput;
            _input.playerMap.PlayerActions.Weapon03.started -= SetWeaponInput;
            _input.playerMap.PlayerActions.Weapon04.started -= SetWeaponInput;
            _input.playerMap.PlayerActions.Weapon05.started -= SetWeaponInput;

            _input.playerMap.PlayerActions.RotateWeapon.started -= TestRotateWeapons;

            cameraLook.enabled = false;
        }

        private void OnDestroy() {
            Health.OnPlayerDeath -= PlayerDeath;
            CutsceneIntroController.OnIntroStartStop -= Intro;

            if (Singleton == this)
                Singleton = null;
        }
        #endregion

        #region Public Methods
        public void PlayShootContinuous(bool value) {
            if (!value) {
                anim.SetBool(AnimProp_IsShooting, false);
                return;
            }

            if (OnShootRequest == null || !OnShootRequest.Invoke())
                return;

            anim.SetBool(AnimProp_IsShooting, true);
        }
        #endregion

        #region Private Methods
        private void Intro(bool ongoing) {
            arms.gameObject.SetActive(!ongoing);

            if (!ongoing)
                SetWeaponType(_currentSelectedWeapon, GetAnimatorIndex(_currentSelectedWeapon));
        }

        private void TestRotateWeapons(InputAction.CallbackContext callbackContext) {
            if (_isDead || !arms.gameObject.activeSelf || Time.timeScale == 0)
                return;

            var directionalIndex = (int)callbackContext.ReadValue<float>();

            _currentSelectedWeapon = (_currentSelectedWeapon + directionalIndex).Cycle(0, 5);
            var animatorIndex = GetAnimatorIndex(_currentSelectedWeapon);

            SetWeaponType(_currentSelectedWeapon, animatorIndex);
        }

        private void GetMovement() {
            _vel.x = _input.GetMovement().x * _currentSpeed;
            _vel.z = _input.GetMovement().y * _currentSpeed;
            _vel = transform.TransformDirection(_vel);

            if (controller.isGrounded && _vel.y < 0)
                _vel.y = -10;
            else
                _vel.y += Physics.gravity.y * 4f * Time.deltaTime;

            _currentSpeed = _isInsideLava ? lavaSpeed : (controller.isGrounded ? speed : airborneSpeed);

            controller.Move(_vel * Time.deltaTime);
        }

        private void Jump(InputAction.CallbackContext ctx) {
            if (!controller.isGrounded || !(_currentCooldownValue <= 0))
                return;

            _vel.y = Mathf.Sqrt((_isInsideLava ? lavaJumpForce : jumpForce) * -3 * Physics.gravity.y);

            RuntimeManager.PlayOneShot(FMODEvents.instance.playerJumpEvent);
        }

        private IEnumerator WalkInLava() {
            _isInsideLava = true;
            ResetJump();

            while (_isInsideLava) {
                Health.GetDamage(lavaDamage);
                yield return new WaitForSeconds(timeDelay);
            }
        }

        private async void ResetJump() {
            if (!_isInsideLava) return;
            _currentCooldownValue = jumpCooldown;

            while (_currentCooldownValue > 0) {
                if (!this)
                    return;

                _currentCooldownValue -= Time.deltaTime;
                await Task.Yield();
            }
        }

        private void ExitLava() {
            StopCoroutine(nameof(WalkInLava));
            _isInsideLava = false;
            _currentCooldownValue = 0;
        }

        private void SetWeaponType(int type, int clip) {
            if (_isDead || !arms.gameObject.activeSelf || Time.timeScale == 0) return;

            anim.ResetTrigger(AnimProp_ShootTrigger);
            anim.SetBool(AnimProp_IsShooting, false);
            VisualSetter.SetHueDeg(hueValue[type].ObjectHue);

            if (anim.GetInteger(AnimProp_WeaponType) == type) return;

            OnWeaponChanged?.Invoke(type);
            anim.SetInteger(AnimProp_WeaponType, type);
            anim.Play(clip);

            _currentSelectedWeapon = type;
        }

        private void PlayShoot(InputAction.CallbackContext ctx) {
            if (_isDead || !arms.gameObject.activeSelf || Time.timeScale == 0)
                return;

            if (OnShootRequest != null && OnShootRequest.Invoke())
                anim.SetTrigger(AnimProp_ShootTrigger);
        }

        private void ShootContinuousInput(InputAction.CallbackContext ctx) => PlayShootContinuous(ctx.started);

        private void SetWeaponInput(InputAction.CallbackContext ctx) {
            if (ctx.action == _input.playerMap.PlayerActions.Weapon01)
                SetWeaponType(0, AnimState_Pistol);
            else if (ctx.action == _input.playerMap.PlayerActions.Weapon02)
                SetWeaponType(1, AnimState_Shotgun);
            else if (ctx.action == _input.playerMap.PlayerActions.Weapon03)
                SetWeaponType(2, AnimState_Rifle);
            else if (ctx.action == _input.playerMap.PlayerActions.Weapon04)
                SetWeaponType(3, AnimState_Frustino);
            else if (ctx.action == _input.playerMap.PlayerActions.Weapon05)
                SetWeaponType(4, AnimState_Sword);
        }

        private void PlayerDeath() {
            _isDead = true;
            Cursor.lockState = CursorLockMode.None;

            if (_isInsideLava)
                ExitLava();

            _input.playerMap.PlayerActions.Jump.started -= Jump;
            _input.playerMap.PlayerActions.Shoot.started -= PlayShoot;
        }

        private void InitializeAudio() => _footstepsInstance = AudioManager.instance.CreateInstance(FMODEvents.instance.playerFootstepsEvent);

        private void UpdateSound() {
            if (controller.isGrounded && controller.velocity.sqrMagnitude > 0.0001f) {
                PLAYBACK_STATE playbackState;
                _footstepsInstance.getPlaybackState(out playbackState);
                if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
                    _footstepsInstance.start();
            }
            else
                _footstepsInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        private int GetAnimatorIndex(int inputIndex) => inputIndex switch {
            0 => AnimState_Pistol,
            1 => AnimState_Shotgun,
            2 => AnimState_Rifle,
            3 => AnimState_Frustino,
            4 => AnimState_Sword,
            _ => 0
        };
        #endregion
    }
}