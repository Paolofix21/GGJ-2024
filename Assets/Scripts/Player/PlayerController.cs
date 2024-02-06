using Code.Graphics;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using FMOD.Studio;
using Barbaragno.RuntimePackages.Operations;
using Miscellaneous;

namespace Code.Player {
    public class PlayerController : MonoBehaviour {
        private bool isDead = false;

        [SerializeField] private ColorSetSO[] hueValue;
        [SerializeField] private GameObject arms;

        #region Properties
        public static PlayerController Singleton { get; set; }
        #endregion

        #region Movement Fields
        [Header("Movement Fields")]
        [SerializeField] private float airborneSpeed = 16f;

        [SerializeField] private float speed = 6f;
        [SerializeField] private float jumpForce = 1.5f;

        private const float grav = -9.8f;
        private float _currentSpeed;

        private int currentSelectedWeapon = default;

        public int CurrentSelectedWeapon => currentSelectedWeapon;

        private EventInstance footsteps_instance;

        private Vector3 vel;

        [SerializeField] private Animator anim;
        private InputManager input;
        #endregion

        #region Player Components Fields
        private CharacterController controller;
        private PlayerView cameraLook;
        [HideInInspector] public PlayerHealth Health;
        public VisualSetter visualSetter { get; private set; }
        #endregion

        #region Lava Fields
        [Header("Lava Fields")]
        [SerializeField] private float lavaSpeed;

        [SerializeField] private float lavaJumpForce = 1.5f;
        [SerializeField] private int timeDelay;
        [SerializeField] private int lavaDamage;

        [SerializeField] private float jumpCooldown;
        private float currentCooldownValue;

        private bool isInsideLava = false;
        #endregion

        #region Animations
        private static readonly int isShooting = Animator.StringToHash("Is Shooting");
        private static readonly int shootTrigger = Animator.StringToHash("Shoot");
        private static readonly int weaponType = Animator.StringToHash("Weapon Type Index");

        private static readonly int Pistol = Animator.StringToHash("Pistol Unholster");
        private static readonly int Rifle = Animator.StringToHash("Rifle Unholster");
        private static readonly int Shotgun = Animator.StringToHash("Shotgun Unholster");
        private static readonly int Frustino = Animator.StringToHash("Frustino Unholster");
        private static readonly int Sword = Animator.StringToHash("Sword Unholster");
        #endregion

        #region Events
        public event Action<int> OnWeaponChanged;
        public event Func<bool> OnShootRequest;
        #endregion

        #region Unity Behaviours
        private void Awake() {
            if (Singleton && Singleton != this) {
                Destroy(gameObject);
                return;
            }

            Singleton = this;

            controller = GetComponent<CharacterController>();
            cameraLook = GetComponent<PlayerView>();
            Health = GetComponent<PlayerHealth>();
            visualSetter = GetComponentInChildren<VisualSetter>();
            input = GetComponent<InputManager>();

            _currentSpeed = speed;
        }

        private void OnEnable() {
            input.playerMap.PlayerActions.Jump.started += Jump;
            input.playerMap.PlayerActions.Shoot.started += PlayShoot;

            input.playerMap.PlayerActions.ContinuousShoot.started += ShootContinuousInput;
            input.playerMap.PlayerActions.ContinuousShoot.canceled += ShootContinuousInput;

            input.playerMap.PlayerActions.Weapon01.started += SetWeaponInput;
            input.playerMap.PlayerActions.Weapon02.started += SetWeaponInput;
            input.playerMap.PlayerActions.Weapon03.started += SetWeaponInput;
            input.playerMap.PlayerActions.Weapon04.started += SetWeaponInput;
            input.playerMap.PlayerActions.Weapon05.started += SetWeaponInput;

            input.playerMap.PlayerActions.RotateWeapon.started += TestRotateWeapons;
        }

        private void Start() {
            CutsceneIntroController.OnIntroStartStop += Intro;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Health.OnPlayerDeath += PlayerDeath;

            InitializeAudio();
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag("Lava"))
                StartCoroutine(WalkInLava());
        }

        private void OnTriggerExit(Collider other) {
            if (other.gameObject.CompareTag("Lava"))
                ExitLava();
        }

        private void Update() {
            if (isDead)
                return;

            GetMovement();
            UpdateSound();
            cameraLook.GetMousePos(input.CameraLookAt());
        }

        private void OnDisable() {
            input.playerMap.PlayerActions.Jump.started -= Jump;
            input.playerMap.PlayerActions.Shoot.started -= PlayShoot;

            input.playerMap.PlayerActions.ContinuousShoot.started -= ShootContinuousInput;
            input.playerMap.PlayerActions.ContinuousShoot.canceled -= ShootContinuousInput;

            input.playerMap.PlayerActions.Weapon01.started -= SetWeaponInput;
            input.playerMap.PlayerActions.Weapon02.started -= SetWeaponInput;
            input.playerMap.PlayerActions.Weapon03.started -= SetWeaponInput;
            input.playerMap.PlayerActions.Weapon04.started -= SetWeaponInput;
            input.playerMap.PlayerActions.Weapon05.started -= SetWeaponInput;

            input.playerMap.PlayerActions.RotateWeapon.started -= TestRotateWeapons;
        }

        private void OnDestroy() {
            Health.OnPlayerDeath -= PlayerDeath;
            CutsceneIntroController.OnIntroStartStop -= Intro;

            if (Singleton == this)
                Singleton = null;
        }

        private void Intro(bool ongoing) {
            arms.gameObject.SetActive(!ongoing);

            if (!ongoing)
                SetWeaponType(currentSelectedWeapon, GetAnimatorIndex(currentSelectedWeapon));
        }
        #endregion

        #region Movement Behaviours
        private void TestRotateWeapons(InputAction.CallbackContext callbackContext) {
            if (isDead || !arms.gameObject.activeSelf || Time.timeScale == 0) return;
            int directionalIndex = (int)callbackContext.ReadValue<float>();

            currentSelectedWeapon = (currentSelectedWeapon + directionalIndex).Cycle(0, 5);
            int animatorIndex = GetAnimatorIndex(currentSelectedWeapon);

            SetWeaponType(currentSelectedWeapon, animatorIndex);
        }

        private void GetMovement() {
            Vector3 dir = Vector3.zero;
            vel.x = input.GetMovement().x * _currentSpeed;
            vel.z = input.GetMovement().y * _currentSpeed;
            vel = transform.TransformDirection(vel);

            if (controller.isGrounded && vel.y < 0) {
                vel.y = -10;
            }
            else {
                vel.y += grav * 4f * Time.deltaTime;
            }

            _currentSpeed = isInsideLava ? lavaSpeed : (controller.isGrounded ? speed : airborneSpeed);

            controller.Move(vel * Time.deltaTime);
        }

        private void Jump(InputAction.CallbackContext ctx) {
            if (controller.isGrounded && currentCooldownValue <= 0) {
                vel.y = Mathf.Sqrt((isInsideLava ? lavaJumpForce : jumpForce) * -3 * grav);

                AudioManager.instance.PlayOneShot(FMODEvents.instance.playerJumpEvent, this.transform.position);
                //if (crouching)
                //    Crouch(ctx, false);
            }
        }

        private IEnumerator WalkInLava() {
            isInsideLava = true;
            ResetJump();

            while (isInsideLava) {
                Health.GetDamage(lavaDamage);
                yield return new WaitForSeconds(timeDelay);
            }
        }

        private async void ResetJump() {
            if (!isInsideLava) return;
            currentCooldownValue = jumpCooldown;

            while (currentCooldownValue > 0) {
                if (!this)
                    return;

                currentCooldownValue -= Time.deltaTime;
                await Task.Yield();
            }
        }

        private void ExitLava() {
            StopCoroutine(nameof(WalkInLava));
            isInsideLava = false;
            currentCooldownValue = 0;
        }
        #endregion

        #region Animation Behaviours
        private void SetWeaponType(int type, int clip) {
            if (isDead || !arms.gameObject.activeSelf || Time.timeScale == 0) return;

            anim.ResetTrigger(shootTrigger);
            anim.SetBool(isShooting, false);
            visualSetter.SetHueDeg(hueValue[type].ObjectHue);

            if (anim.GetInteger(weaponType) == type) return;

            OnWeaponChanged?.Invoke(type);
            anim.SetInteger(weaponType, type);
            anim.Play(clip);

            if (currentSelectedWeapon != type) currentSelectedWeapon = type;
        }

        private void PlayShoot(InputAction.CallbackContext ctx) {
            if (isDead || !arms.gameObject.activeSelf || Time.timeScale == 0) return;
            if (OnShootRequest != null && OnShootRequest.Invoke())
                anim.SetTrigger(shootTrigger);
        }

        public void PlayShootContinuous(bool _value) {
            if (!_value) {
                anim.SetBool(isShooting, false);
                return;
            }

            if (OnShootRequest == null || !OnShootRequest.Invoke())
                return;

            anim.SetBool(isShooting, true);
        }

        private void ShootContinuousInput(InputAction.CallbackContext ctx) => PlayShootContinuous(ctx.started);

        private void SetWeaponInput(InputAction.CallbackContext ctx) {
            if (ctx.action == input.playerMap.PlayerActions.Weapon01)
                SetWeaponType(0, Pistol);
            else if (ctx.action == input.playerMap.PlayerActions.Weapon02)
                SetWeaponType(1, Shotgun);
            else if (ctx.action == input.playerMap.PlayerActions.Weapon03)
                SetWeaponType(2, Rifle);
            else if (ctx.action == input.playerMap.PlayerActions.Weapon04)
                SetWeaponType(3, Frustino);
            else if (ctx.action == input.playerMap.PlayerActions.Weapon05)
                SetWeaponType(4, Sword);
        }
        #endregion

        #region Death Behaviours
        private void PlayerDeath() {
            isDead = true;
            Cursor.lockState = CursorLockMode.None;

            if (isInsideLava)
                ExitLava();

            input.playerMap.PlayerActions.Jump.started -= Jump;
            input.playerMap.PlayerActions.Shoot.started -= PlayShoot;
        }
        #endregion

        #region Audio
        private void InitializeAudio() => footsteps_instance = AudioManager.instance.CreateInstance(FMODEvents.instance.playerFootstepsEvent);

        private void UpdateSound() {
            if (controller.isGrounded && controller.velocity.sqrMagnitude > 0.0001f) {
                PLAYBACK_STATE playbackState;
                footsteps_instance.getPlaybackState(out playbackState);
                if (playbackState.Equals(PLAYBACK_STATE.STOPPED)) {
                    footsteps_instance.start();
                }
            }
            else {
                footsteps_instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
        }
        #endregion

        #region Utility
        private int GetAnimatorIndex(int inputIndex) => inputIndex switch {
            0 => Pistol,
            1 => Shotgun,
            2 => Rifle,
            3 => Frustino,
            4 => Sword,
            _ => 0
        };
        #endregion
    }
}