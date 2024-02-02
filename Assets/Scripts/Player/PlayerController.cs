using FMODUnity;
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
        [SerializeField] private EndGameUI endgameUI;
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
        private bool crouching = false;

        private int currentSelectedWeapon = default;

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
        private void Start() {
            CutsceneIntroController.OnIntroStartStop += Intro;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            input.playerMap.PlayerActions.Jump.started += Jump;
            input.playerMap.PlayerActions.Shoot.started += PlayShoot;

            input.playerMap.PlayerActions.ContinuousShoot.started += _ => PlayShootContinuous(true);
            input.playerMap.PlayerActions.ContinuousShoot.canceled += _ => PlayShootContinuous(false);

            input.playerMap.PlayerActions.Weapon01.started += _ => SetWeaponType(0, Pistol);
            input.playerMap.PlayerActions.Weapon02.started += _ => SetWeaponType(1, Shotgun);
            input.playerMap.PlayerActions.Weapon03.started += _ => SetWeaponType(2, Rifle);
            input.playerMap.PlayerActions.Weapon04.started += _ => SetWeaponType(3, Frustino);
            input.playerMap.PlayerActions.Weapon05.started += _ => SetWeaponType(4, Sword);

            input.playerMap.PlayerActions.RotateWeapon.started += TestRotateWeapons;

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

        private void OnDestroy() {
            input.playerMap.PlayerActions.Jump.started -= Jump;
            input.playerMap.PlayerActions.Shoot.started -= PlayShoot;

            Health.OnPlayerDeath -= PlayerDeath;
            CutsceneIntroController.OnIntroStartStop -= Intro;

            if (Singleton == this)
                Singleton = null;
        }

        private void Intro(bool ongoing) 
        { 
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
            
            if (controller.isGrounded && currentCooldownValue <=0) {
                vel.y = Mathf.Sqrt((isInsideLava ? lavaJumpForce : jumpForce) * -3 * grav);

                AudioManager.instance.PlayOneShot(FMODEvents.instance.playerJumpEvent, this.transform.position);
                //if (crouching)
                //    Crouch(ctx, false);
            }
        }

        #region Crouching [NOT USED]
        //private void Crouch(InputAction.CallbackContext ctx) {
        //    Crouch(ctx, true);
        //}

        //private void Crouch(InputAction.CallbackContext ctx, bool shouldReproSFX = true) {
        //    if (isInsideLava)
        //        return;

        //    crouching = !crouching;
        //    cameraLook.ChangeViewHeight(crouching);

        //    if (shouldReproSFX) {
        //        PlayCrouchSound();
        //    }
        //}

        //private void PlayCrouchSound() {
        //    AudioManager.instance.PlayOneShot(FMODEvents.instance.playerCrouchEvent, this.transform.position);
        //}
        #endregion

        private IEnumerator WalkInLava() {
            isInsideLava = true;
            ResetJump();

            //if (crouching) {
            //    crouching = !crouching;
            //    cameraLook.ChangeViewHeight(crouching);
            //}

            while (isInsideLava) {
                Health.GetDamage(lavaDamage);
                yield return new WaitForSeconds(timeDelay);
            }
        }

        private async void ResetJump()
        {
            if (!isInsideLava) return;
            currentCooldownValue = jumpCooldown;

            while(currentCooldownValue > 0) 
            {
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

            if(currentSelectedWeapon != type) currentSelectedWeapon = type;
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
        #endregion
        EndGameUI endgame;
        #region Death Behaviours
        private void PlayerDeath() {
            isDead = true;
            Cursor.lockState = CursorLockMode.None;

            if (endgame == null)
            {
                endgame = Instantiate(endgameUI);
                endgame.CallEndgame(EndGameUI.EndgameState.GameOver);
            }

            if (isInsideLava)
                ExitLava();
        }
        #endregion

        #region Audio
        private void InitializeAudio() {
            footsteps_instance = AudioManager.instance.CreateInstance(FMODEvents.instance.playerFootstepsEvent);
        }

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
        private int GetAnimatorIndex(int inputIndex) {
            switch (inputIndex) {
                case 0:
                    return Pistol;
                case 1:
                    return Shotgun;
                case 2:
                    return Rifle;
                case 3:
                    return Frustino;
                case 4:
                    return Sword;
                default:
                    return 0;
            }
        }
        #endregion
    }
}
