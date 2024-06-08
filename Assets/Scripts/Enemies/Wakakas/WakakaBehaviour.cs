using Code.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.EnemySystem.Wakakas {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(WakakaMaskAnimator))]
    [RequireComponent(typeof(WakakaHealth))]
    [RequireComponent(typeof(BoxCollider))]
    public class WakakaBehaviour : MonoBehaviour {
        private enum WakakaState {
            None,
            Wander,
            Chase,
            Flee
        }

        #region Public Variables
        [Header("Settings")]
        [SerializeField] private float m_wanderSpeed = 2f;
        [SerializeField] private float m_chaseSpeed = 2f;
        [SerializeField] private float m_fleeSpeed = 2f;
        [SerializeField] private Vector2 m_wanderRefreshTimeRange = new(2f, 4f);
#if UNITY_EDITOR
        public bool debugState;
#endif

        [Space]
        [SerializeField] private float m_detectionDistance = 2f;
        [SerializeField] private bool m_chaseSinceStart;
        [SerializeField] private bool m_alwaysFaceTarget;
        [SerializeField] private bool m_characterPredictPosition;

        [Space]
        [SerializeField] private float m_wanderLerpQuickness = 8f;
        [SerializeField] private float m_chaseLerpQuickness = 4f;

        [Space]
        [SerializeField, Min(5f)] private float m_maxDistanceFromPlayer = 30f;
        [SerializeField, Min(5f)] private float m_maxDistanceYFromPlayer = 8f;
        [SerializeField, Min(1f)] private float m_maxWallCastDistance = 3f;
        [SerializeField] private LayerMask m_wallMask = ~0;

        [Space]
        [SerializeField] private SteamIntegration.Statistics.SteamStatisticSO m_enemiesKilledStat;

        public event System.Action<WakakaBehaviour> OnDeath;
        #endregion

        #region Private Variables
        private Rigidbody _body;
        private BoxCollider _collider;
        private WakakaMaskAnimator _maskAnimator;
        private WakakaHealth _health;
        private WakakaAttacker _attacker;

        private Transform _target;

        private WakakaState _state = WakakaState.None;
        private Vector3 _moveDirection;

#if UNITY_EDITOR
        private TextMesh _textState;
#endif

        private System.Action _logic;
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            _body = GetComponent<Rigidbody>();
            _collider = GetComponent<BoxCollider>();
            _health = GetComponent<WakakaHealth>();
            _attacker = GetComponentInChildren<WakakaAttacker>();
            _maskAnimator = GetComponent<WakakaMaskAnimator>();

            _health.OnHealthChanged += OnHealthChanged;
            _health.OnDeath += OnDie;

#if UNITY_EDITOR
            if (!debugState)
                return;

            var obj = new GameObject("State Text");
            obj.transform.SetParent(transform);
            obj.transform.localPosition = Vector3.up;
            obj.transform.localRotation = Quaternion.identity;
            _textState = obj.AddComponent<TextMesh>();
            _textState.alignment = TextAlignment.Center;
            _textState.anchor = TextAnchor.MiddleCenter;
#endif
        }

        private void OnEnable() {
            _body.isKinematic = false;
            _health.enabled = true;
            if (_attacker)
                _attacker.enabled = true;
            RefreshState();
        }

        private void Start() {
            _maskAnimator.AnimateIntroVoiceLine();

            _target = GameEvents.MatchManager.GetPlayerEntity().Transform;

            SetState(WakakaState.Wander);

            if (m_chaseSinceStart)
                ForceChasePlayer();
        }

        private void FixedUpdate() {
#if UNITY_EDITOR
            if (_textState && _target)
                _textState.transform.LookAt(_target);
#endif

            switch (_state) {
                case WakakaState.Wander:
                    Wander();
                    break;
                case WakakaState.Chase:
                    Chase();
                    break;
                case WakakaState.Flee:
                    Flee();
                    break;
                default:
                    return;
            }
        }

        private void OnDrawGizmos() => Gizmos.DrawRay(transform.position, _moveDirection * m_maxWallCastDistance);

        private void OnDisable() {
            _body.isKinematic = true;
            _health.enabled = false;
            if (_attacker)
                _attacker.enabled = false;

            CancelInvoke();
#if UNITY_EDITOR
            if (_textState)
                _textState.text = null;
#endif
        }
        #endregion

        #region Public Methods
        public void ForceChasePlayer() => SetState(WakakaState.Chase);
        #endregion

        #region Private Methods
        private void SetState(WakakaState state) {
            if (_state == state)
                return;

            CancelInvoke();

            _state = state;
            RefreshState();
        }

        private void RefreshState() {
#if UNITY_EDITOR
            if (_textState)
                _textState.text = _state.ToString();
#endif

            switch (_state) {
                case WakakaState.Wander:
                    _moveDirection = transform.forward;
                    Invoke(nameof(RefreshWander), 1f);
                    InvokeRepeating(nameof(CheckDistancePeriodically), 1f, 1f);
                    break;
                case WakakaState.Chase:
                    break;
                case WakakaState.Flee:
                    _moveDirection = transform.position - _target.position;
                    _moveDirection.Normalize();
                    break;
                default:
                    return;
            }
        }

        private void RefreshWander() {
            _moveDirection = Random.onUnitSphere;
            _moveDirection.y *= .5f;
            _moveDirection.Normalize();
            // DoRayCastAndAdjustDirection(ref _moveDirection);
            Invoke(nameof(RefreshWander), Random.Range(m_wanderRefreshTimeRange.x, m_wanderRefreshTimeRange.y));
        }

        private void Wander() {
            if (Physics.BoxCast(transform.position, _collider.size * .4f, _moveDirection, out var hit, transform.rotation, m_maxWallCastDistance))_moveDirection = Quaternion.Euler(transform.right * 90f) * hit.normal; // Bend upwards

            _body.velocity = Vector3.Slerp(_body.velocity, _moveDirection * m_wanderSpeed, Time.deltaTime * m_wanderLerpQuickness);
            transform.forward = Vector3.Slerp(transform.forward, _moveDirection, Time.deltaTime * m_wanderLerpQuickness * .5f);
        }

        private void Chase() {
            var pos = transform.position;
            var targetPos = _target.position;

            _moveDirection = (targetPos - pos).normalized;

            if (Physics.BoxCast(pos, _collider.size * .4f, _moveDirection, out var hit, transform.rotation, m_maxWallCastDistance)) {
                if (!hit.collider.CompareTag("Player"))
                    _moveDirection = Quaternion.Euler(transform.right * 90f) * hit.normal; // Bend upwards
            }

            _body.velocity = Vector3.Slerp(_body.velocity, _moveDirection * m_chaseSpeed, Time.deltaTime * m_chaseLerpQuickness);

            if (m_alwaysFaceTarget) {
                if (m_characterPredictPosition && _target.TryGetComponent(out Rigidbody body))
                    targetPos += body.velocity * Time.deltaTime;

                transform.LookAt(targetPos);
            }
            else
                transform.forward = Vector3.Slerp(transform.forward, _moveDirection, Time.deltaTime * m_chaseLerpQuickness * .5f);
        }

        private void Flee() => _body.velocity = _moveDirection * m_fleeSpeed;

        private void CheckDistancePeriodically() {
            var pos = transform.position;
            var playerPos = _target.position;
            var distFromPlayer = Vector3.Distance(pos, playerPos);

            var verticalDistance = Mathf.Abs(pos.y - playerPos.y);

            if (distFromPlayer < m_detectionDistance) {
                SetState(WakakaState.Chase);
                _maskAnimator.AnimateLaughter();
                return;
            }

            if (distFromPlayer < m_maxDistanceFromPlayer && verticalDistance < m_maxDistanceYFromPlayer)
                return;

            _moveDirection = _target.position - transform.position;
            _moveDirection.Normalize();
        }

        /*private void DoRayCastAndAdjustDirection(ref Vector3 direction) {
            // if (!Physics.SphereCast(transform.position, .25f, direction, out var hit, m_maxWallCastDistance))
            if (!Physics.BoxCast(transform.position, _collider.size * .4f, direction, out var hit, transform.rotation, m_maxWallCastDistance))
                return;

            direction = hit.normal;
        }*/
        #endregion

        #region Event Methods
        private void OnHealthChanged(float health) {
            if (health < 1f)
                _maskAnimator.AnimateDamage();
        }

        private void OnDie() {
            _collider.enabled = false;
            if (_attacker)
                Destroy(_attacker.gameObject);
            SetState(WakakaState.Flee);

            _maskAnimator.AnimateDeath();
            OnDeath?.Invoke(this);

            SteamIntegration.Statistics.SteamStatisticsController.Singleton?.AdvanceStat(m_enemiesKilledStat, 1);
            SteamIntegration.Statistics.SteamStatisticsController.Singleton?.PushStats();
        }
        #endregion
    }
}