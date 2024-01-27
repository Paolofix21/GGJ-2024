using FMODUnity;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using FMOD.Studio;

namespace Code.Player
{
    public class PlayerController : MonoBehaviour
    {
        private bool isDead = false;

        #region Movement Fields
        [Header("Movement Fields")]
        [SerializeField] private float speed = 6f; 
        [SerializeField] private float jumpForce = 1.5f;

        private const float grav = -9.8f;
        private bool crouching = false;

        private EventInstance footsteps_instance;

        private Vector3 vel;

        [SerializeField] private Animator anim;
        private InputManager input;
        #endregion

        #region Player Components Fields
        private CharacterController controller;
        private Rigidbody rigidbody;
        private PlayerView cameraLook;
        private PlayerHealth health;
        #endregion

        #region Lava Fields
        [Header("Lava Fields")]
        [SerializeField] private float lavaSpeed;
        [SerializeField] private int timeDelay;
        [SerializeField] private int lavaDamage;

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
        #endregion

        #region Unity Behaviours

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            controller = GetComponent<CharacterController>();
            cameraLook = GetComponent<PlayerView>();
            health = GetComponent<PlayerHealth>();
            input = GetComponent<InputManager>();

            input.playerMap.PlayerActions.Jump.performed +=  Jump;
            input.playerMap.PlayerActions.Crouch.performed += Crouch;
            input.playerMap.PlayerActions.Shoot.performed += PlayShoot;
            input.playerMap.PlayerActions.ContinuousShoot.performed += _ => PlayShootContinuous(true);
            input.playerMap.PlayerActions.ContinuousShoot.canceled += _ => PlayShootContinuous(false);

            input.playerMap.PlayerActions.Weapon01.performed += _ =>  SetWeaponType(0, Pistol);
            input.playerMap.PlayerActions.Weapon02.performed += _ =>  SetWeaponType(1, Shotgun);
            input.playerMap.PlayerActions.Weapon03.performed += _ =>  SetWeaponType(2, Rifle);
            input.playerMap.PlayerActions.Weapon04.performed += _ =>  SetWeaponType(3,Frustino);
            input.playerMap.PlayerActions.Weapon05.performed += _ =>  SetWeaponType(4, Sword);

            health.OnPlayerDeath += PlayerDeath;

            InitializeAudio();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Lava"))
                StartCoroutine(WalkInLava());
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Lava"))
                ExitLava();
        }

        private void Update()
        {
            if (isDead)
                return;

            GetMovement();
            UpdateSound();
            cameraLook.GetMousePos(input.CameraLookAt());
        }

        private void OnDestroy()
        {
            input.playerMap.PlayerActions.Jump.performed -= Jump;
            input.playerMap.PlayerActions.Crouch.performed -= Crouch;
            input.playerMap.PlayerActions.Shoot.performed -= PlayShoot;

            health.OnPlayerDeath -= PlayerDeath;
        }
        #endregion

        #region Movement Behaviours
        private void GetMovement()
        {
            Vector3 dir = Vector3.zero;
            dir.x = input.GetMovement().x;
            dir.z = input.GetMovement().y;

            if (controller.isGrounded && vel.y < 0)
            {
                vel.y = -2;
            }
            else
            {
                vel.y += grav * Time.deltaTime;
            }

            controller.Move(transform.TransformDirection(dir) * speed * Time.deltaTime);
            controller.Move(vel * Time.deltaTime);
        }

        private void Jump(InputAction.CallbackContext ctx)
        {
            Debug.Log(controller.isGrounded);
            if (controller.isGrounded)
            {
                vel.y = Mathf.Sqrt(jumpForce * -3 * grav);
                
                AudioManager.instance.PlayOneShot(FMODEvents.instance.playerJumpEvent, this.transform.position);

                if (crouching)
                    Crouch(ctx, false);
            }
        }

        private void Crouch(InputAction.CallbackContext ctx)
        {
            Crouch(ctx, true);
        }

        private void Crouch(InputAction.CallbackContext ctx, bool shouldReproSFX = true)
        {
            if (isInsideLava)
                return;

            crouching = !crouching;
            cameraLook.ChangeViewHeight(crouching);

            if (shouldReproSFX)
            {
                PlayCrouchSound();
            }
        }

        private void PlayCrouchSound()
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.playerCrouchEvent, this.transform.position);
        }

        private IEnumerator WalkInLava()
        {
            isInsideLava = true;
            speed = lavaSpeed;
            while (isInsideLava)
            {
                health.GetDamage(lavaDamage);
                yield return new WaitForSeconds(timeDelay);
            }
        }

        private void ExitLava()
        {
            StopCoroutine("WalkInLava");
            isInsideLava = false;
            speed = 6f;
        }
        #endregion

        #region Animation Behaviours
        private void SetWeaponType(int type, int clip)
        {
            OnWeaponChanged?.Invoke(type);
            anim.SetInteger(weaponType, type);
            anim.Play(clip);
        }

        private void PlayShoot(InputAction.CallbackContext ctx)
        {
            anim.SetTrigger(shootTrigger);
        }

        private void PlayShootContinuous(bool _value)
        {
            if (_value && !anim.GetBool(isShooting))
                anim.SetBool(isShooting, true);
            else if(!_value && anim.GetBool(isShooting))
                anim.SetBool(isShooting, false);
        }
        #endregion

        #region Death Behaviours
        private void PlayerDeath()
        {
            isDead = true;
            Cursor.lockState = CursorLockMode.None;

            if (isInsideLava)
                ExitLava();
        }
        #endregion

        #region Audio
        private void InitializeAudio()
        {
            footsteps_instance = AudioManager.instance.CreateInstance(FMODEvents.instance.playerFootstepsEvent);
        }

        private void UpdateSound()
        {
            if(controller.isGrounded && controller.velocity != Vector3.zero)
            {
                PLAYBACK_STATE playbackState;
                footsteps_instance.getPlaybackState(out playbackState);
                if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
                {
                    footsteps_instance.start();
                }
            }
            else
            {
                footsteps_instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
        }
        #endregion
    }
}

