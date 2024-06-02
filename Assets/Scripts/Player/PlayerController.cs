using Code.Graphics;
using System;
using System.Collections;
using System.Threading.Tasks;
using Audio;
using UnityEngine;
using Barbaragno.RuntimePackages.Operations;
using Code.Core;
using Izinspector.Runtime.PropertyAttributes;
using Miscellaneous;
using SteamIntegration.Achievements;
using SteamIntegration.Statistics;
using Utilities;

namespace Code.Player {
    [DefaultExecutionOrder(1)]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerController : MonoBehaviour {
        #region Public Variables
        [Header("References")]
        [SerializeField] private ColorSetSO[] hueValue;
        [SerializeField] private GameObject arms;
        [SerializeField] private Animator anim;

        [Header("Audio")]
        [SerializeField] private SoundSO m_jumpSound;
        [SerializeField] private SoundSO m_footStepsSound;
        [SerializeField] private SoundSO m_trampulineSound;

        [Header("Movement Fields")]
        [SerializeField] private float airborneSpeed = 16f;
        [SerializeField] private float lavaSpeed = 8f;
        [SerializeField] private float speed = 6f;
        [Space(2)]
        [SerializeField] private float jumpForce = 1.5f;

        [Header("Settings")]
        [SerializeField] private float m_groundNormalThresholdY = 0.35f;
        [SerializeField] private float m_dragGrounded = 6f;
        [SerializeField] private float m_dragAirborne = 2f;
        [SerializeField] private float m_gravityScale = 2f;
        [SerializeField] private float m_coyoteTime = .15f;

        [Space]
        [SerializeField] private float m_padForceMultiplier = 500f;
        [SerializeField] private float m_jumpPadVerticalPredominance = 2f;

        [Header("Lava Fields")]
        [SerializeField] private float lavaJumpForce = 1.5f;
        [SerializeField] private float lavaDamageDelay = 2f;
        [SerializeField] private int lavaDamage = 5;
        [Space(2)]
        [SerializeField] private float jumpCooldown = 1.5f;

        [Header("Achievements")]
        [SerializeField] private SteamStatisticSO m_mushroomJumpStat;
        [SerializeField] private SteamStatisticSO m_lavaTimeStat;
        [SerializeField] private SteamAchievementSO m_deathByTrapezioAchievement;

        [Space]
        [SerializeField, Tag] private string lavaLayer;

        public event Action<int> OnWeaponChanged;
        public event Func<bool> OnShootRequest;
        #endregion

        #region Properties
        public PlayerHealth Health { get; private set; }
        public VisualSetter VisualSetter { get; private set; }
        public bool IsGrounded { get; private set; }
        #endregion

        #region Private Variables
        private Rigidbody _body;
        private CapsuleCollider _collider;
        private PlayerView _cameraLook;
        private PlayerInputController _playerInput;

        private SoundPlayer _footstepsInstance;
        private Coroutine _inLavaCoroutine;

        private bool _isDead;
        private bool _isInsideLava;
        private float _timeEnterLava;

        private bool _wasGrounded;
        private bool _didJump;
        private float _coyoteTime;

        private Vector3 _vel;
        private float _currentSpeed;
        private RaycastHit _groundInfo;

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
            Health = GetComponent<PlayerHealth>();
            VisualSetter = GetComponentInChildren<VisualSetter>();

            _body = GetComponent<Rigidbody>();
            _collider = GetComponent<CapsuleCollider>();
            _cameraLook = GetComponentInChildren<PlayerView>();
            _playerInput = GetComponent<PlayerInputController>();

            _currentSpeed = speed;

            CutsceneIntroController.OnIntroStartStop += Intro;
            GameEvents.OnPauseStatusChanged += Pause;
        }

        private void OnEnable() {
            _playerInput.OnJump += Jump;
            _playerInput.OnShoot += PlayShoot;
            _playerInput.OnShootStateChange += ShootContinuousInput;
            _playerInput.OnWeaponIndexChange += SetWeaponInput;
            _playerInput.OnCycleWeapons += CycleWeapons;

            Health.enabled = true;
            _cameraLook.enabled = true;
        }

        private void Start() {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Health.OnPlayerDeath += PlayerDeath;

            InitializeAudio();
        }

        private void Update() {
            if (_isDead)
                return;

            IsGrounded = CheckGround();

            if (IsGrounded != _wasGrounded && _wasGrounded) {
                if (_didJump)
                    _didJump = false;
                else
                    _coyoteTime = Time.time + m_coyoteTime;
            }

            _cameraLook.ApplyMotion(_playerInput.LookInput);

            _wasGrounded = IsGrounded;
        }

        private void FixedUpdate() {
            if (_isDead)
                return;

            _body.drag = IsGrounded ? m_dragGrounded : m_dragAirborne;

            Move();
        }

        private void LateUpdate() {
            if (_isDead)
                return;

            UpdateSound();
        }

        private void OnCollisionEnter(Collision other) {
            if (!other.gameObject.TryGetComponent(out JumpPad jumpPad))
                return;

            MushroomJump(jumpPad, other.contacts[0]);
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag(lavaLayer))
                EnterLava();
        }

        private void OnTriggerExit(Collider other) {
            if (other.gameObject.CompareTag(lavaLayer))
                ExitLava();
        }

        private void OnDrawGizmos() {
            _collider = GetComponent<CapsuleCollider>();
            var origin = transform.position + _collider.center - Vector3.up * (_collider.height * .5f - _collider.radius);
            Gizmos.DrawSphere(origin, _collider.radius - .1f);
        }

        private void OnDisable() {
            _playerInput.OnJump -= Jump;
            _playerInput.OnShoot -= PlayShoot;
            _playerInput.OnShootStateChange -= ShootContinuousInput;
            _playerInput.OnWeaponIndexChange -= SetWeaponInput;
            _playerInput.OnCycleWeapons -= CycleWeapons;

            PlayShootContinuous(false);
            Health.enabled = false;
            _cameraLook.enabled = false;

            _footstepsInstance.Stop(StopMode.Sudden);
        }

        private void OnDestroy() {
            Health.OnPlayerDeath -= PlayerDeath;
            GameEvents.OnPauseStatusChanged -= Pause;
            CutsceneIntroController.OnIntroStartStop -= Intro;
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
            enabled = !ongoing;

            if (!ongoing)
                SetWeaponType(_currentSelectedWeapon, GetAnimatorIndex(_currentSelectedWeapon));
        }

        private void CycleWeapons(int direction) {
            if (_isDead || !arms.gameObject.activeSelf || Time.timeScale == 0)
                return;

            _currentSelectedWeapon = (_currentSelectedWeapon + direction).Cycle(0, 5);
            var animatorIndex = GetAnimatorIndex(_currentSelectedWeapon);

            SetWeaponType(_currentSelectedWeapon, animatorIndex);
        }

        private bool CheckGround() {
            var origin = transform.position + _collider.center - Vector3.up * (_collider.height * .5f - _collider.radius);

            if (!Physics.SphereCast(origin, _collider.radius - .1f, Vector3.down, out var hit, .15f))
                return false;

            _groundInfo = hit;
            return hit.normal.y > m_groundNormalThresholdY;
        }

        private void Move() {
            if (IsGrounded)
                _body.AddForce(-_groundInfo.normal * 5f, ForceMode.VelocityChange);
            else
                _body.AddForce(Physics.gravity * m_gravityScale, ForceMode.Acceleration);

            // if (_playerInput.MoveInput.sqrMagnitude < .01f)
            //     return;

            _currentSpeed = _isInsideLava ? lavaSpeed : (IsGrounded ? speed : airborneSpeed);

            var inputDirection = new Vector3 {
                x = _playerInput.MoveInput.x * _currentSpeed,
                z = _playerInput.MoveInput.y * _currentSpeed
            };

            inputDirection = _cameraLook.transform.TransformDirection(inputDirection);
            inputDirection.y = 0;
            inputDirection.Normalize();

            _vel = inputDirection * _currentSpeed;

            _body.AddForce(_vel, ForceMode.Acceleration);

            if (IsGrounded)
                return;

            // DRAG
            // velocity = velocity * (1 - deltaTime * drag)
            var velocityY = _body.velocity.y * _body.drag * Time.deltaTime;
            var force = Physics.gravity.y * Time.deltaTime;
            var bodyDrag = 1 - Time.deltaTime * _body.drag;
 
            _body.AddForce(new Vector3 (0, (velocityY + force) / bodyDrag, 0), ForceMode.VelocityChange);
        }

        private void Jump() {
            if (_didJump)
                return;

            if ((!IsGrounded && _coyoteTime < Time.time) || _currentCooldownValue > 0)
                return;

            if (IsGrounded)
                _didJump = true;

            var velocity = _body.velocity;
            velocity.y = 0;
            _body.velocity = velocity;
            _body.AddForce((_isInsideLava ? lavaJumpForce : jumpForce) * _body.mass * Vector3.up, ForceMode.Impulse);

            AudioManager.Singleton.PlayOneShotWorldAttached(m_jumpSound.GetSound(), gameObject, MixerType.Voice);
        }

        private void MushroomJump(JumpPad jumpPad, ContactPoint point) {
            var normal = point.normal;
            if (normal.y < .5f)
                return;

            var inputDirection = new Vector3(_playerInput.MoveInput.x, 0f, _playerInput.MoveInput.y);
            var jumpDirection = _cameraLook.transform.TransformDirection(inputDirection).normalized;
            jumpDirection += Vector3.up * m_jumpPadVerticalPredominance;
            jumpDirection.Normalize();

            _body.velocity = Vector3.zero;
            _body.AddForce(jumpDirection * jumpPad.PushSpeed * m_padForceMultiplier, ForceMode.Impulse);
            AudioManager.Singleton.PlayOneShotWorld(m_trampulineSound.GetSound(), jumpPad.transform.position, MixerType.SoundFx);

            SteamStatisticsController.Singleton?.AdvanceStat(m_mushroomJumpStat, 1);
            SteamStatisticsController.Singleton?.PushStats();
        }

        private IEnumerator WalkInLava() {
            _isInsideLava = true;

            ResetJump();
            var delayHalf = new WaitForSeconds(lavaDamageDelay * .5f);

            while (_isInsideLava) {
                if (GameEvents.IsOnHold)
                    yield return null;

                yield return delayHalf;
                Health.DealDamage(lavaDamage, gameObject);
                yield return delayHalf;
            }
        }

        private async void ResetJump() {
            if (!_isInsideLava)
                return;

            _currentCooldownValue = jumpCooldown;

            while (_currentCooldownValue > 0) {
                if (!this)
                    return;

                _currentCooldownValue -= Time.deltaTime;
                await Task.Yield();
            }
        }

        private void EnterLava() {
            _inLavaCoroutine = StartCoroutine(WalkInLava());
            _timeEnterLava = Time.time;
        }

        private void ExitLava() {
            StopCoroutine(_inLavaCoroutine);
            _isInsideLava = false;
            _currentCooldownValue = 0;
            SteamStatisticsController.Singleton?.AdvanceStat(m_lavaTimeStat, Time.time - _timeEnterLava);
            SteamStatisticsController.Singleton?.PushStats();
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

        private void PlayShoot() {
            if (_isDead || !arms.gameObject.activeSelf || Time.timeScale == 0)
                return;

            if (OnShootRequest != null && OnShootRequest.Invoke())
                anim.SetTrigger(AnimProp_ShootTrigger);
        }

        private void ShootContinuousInput(bool started) => PlayShootContinuous(started);

        private void SetWeaponInput(int index) {
            var animationHash = index switch {
                0 => AnimState_Pistol,
                1 => AnimState_Shotgun,
                2 => AnimState_Rifle,
                3 => AnimState_Frustino,
                4 => AnimState_Sword,
                _ => 0
            };

            SetWeaponType(index, animationHash);
        }

        private void PlayerDeath() {
            _isDead = true;
            Cursor.lockState = CursorLockMode.None;

            if (_isInsideLava)
                ExitLava();

            if (Health.DamageObject == DamageObject.Trapezio)
                SteamAchievementsController.Singleton?.AdvanceAchievement(m_deathByTrapezioAchievement);
        }

        private void InitializeAudio() => _footstepsInstance = AudioManager.Singleton.CreateSource(m_footStepsSound, MixerType.SoundFx, gameObject);

        private void UpdateSound() {
            if (IsGrounded && _body.velocity.sqrMagnitude > 0.0001f) {
                if (!_footstepsInstance.IsPlaying)
                    _footstepsInstance.Play();
            }
            else if (_footstepsInstance.IsPlaying)
                _footstepsInstance.Stop(StopMode.Sudden);
        }

        private int GetAnimatorIndex(int inputIndex) => inputIndex switch {
            0 => AnimState_Pistol,
            1 => AnimState_Shotgun,
            2 => AnimState_Rifle,
            3 => AnimState_Frustino,
            4 => AnimState_Sword,
            _ => 0
        };

        private void Pause(bool value) => anim.speed = value ? 0f : 1f;
        #endregion
    }
}