using Code.Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.EnemySystem.Wakakas {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(WakakaMaskAnimator))]
    [RequireComponent(typeof(WakakaHealth))]
    public class WakakaBehaviour : MonoBehaviour {
        private enum WakakaState {
            None,
            Wander,
            Chase,
            Flee
        }

        #region Public Variables
        [Header("Settings")]
        [SerializeField] public float m_wanderSpeed = 2f;
        [SerializeField] public float m_chaseSpeed = 2f;
        [SerializeField] public float m_fleeSpeed = 2f;

        [Space]
        [SerializeField] public float m_detectionDistance = 2f;

        [Space]
        [SerializeField] public float m_wanderLerpQuickness = 8f;
        [SerializeField] public float m_chaseLerpQuickness = 4f;

        [Space]
        [SerializeField, Min(5f)] public float m_maxDistanceFromPlayer = 30f;
        [SerializeField, Min(5f)] public float m_maxDistanceYFromPlayer = 8f;
        [SerializeField, Min(1f)] public float m_maxWallCastDistance = 3f;

        public event System.Action<WakakaBehaviour> OnDeath;
        #endregion

        #region Private Variables
        private Rigidbody _body;
        private Collider _collider;
        private WakakaMaskAnimator _maskAnimator;
        private WakakaHealth _health;
        private WakakaAttacker _attacker;

        private System.Action _logic;

        private WakakaState _state = WakakaState.None;
        private Vector3 _moveDirection;

        private static event System.Action OnEveryoneChasePlayer;
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            _body = GetComponent<Rigidbody>();
            _health = GetComponent<WakakaHealth>();
            _attacker = GetComponentInChildren<WakakaAttacker>();
            _maskAnimator = GetComponent<WakakaMaskAnimator>();

            _health.OnDeath += OnDie;
        }

        private void Start() {
            _maskAnimator.AnimateIntroVoiceLine();

            SetState(WakakaState.Wander);

            OnEveryoneChasePlayer += ForceChasePlayer;
        }

        private void FixedUpdate() {
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

        private void OnDestroy() => OnEveryoneChasePlayer -= ForceChasePlayer;
        #endregion

        #region Private Methods
        private void SetState(WakakaState state) {
            if (_state == state)
                return;

            CancelInvoke();

            _state = state;

            switch (_state) {
                case WakakaState.Wander:
                    RefreshWander();
                    InvokeRepeating(nameof(CheckDistancePeriodically), 1f, 1f);
                    break;
                case WakakaState.Chase:
                    break;
                case WakakaState.Flee:
                    _moveDirection = (transform.position - PlayerController.Singleton.transform.position).normalized;
                    break;
                default:
                    return;
            }
        }

        private void RefreshWander() {
            _moveDirection = Random.onUnitSphere;
            _moveDirection.y *= .5f;
            _moveDirection.Normalize();
            DoRayCastAndAdjustDirection(ref _moveDirection);
            Invoke(nameof(RefreshWander), Random.Range(2, 4f));
        }

        private void Wander() {
            _body.velocity = Vector3.Slerp(_body.velocity, _moveDirection * m_wanderSpeed, Time.deltaTime * m_wanderLerpQuickness);
            transform.forward = Vector3.Slerp(transform.forward, _moveDirection, Time.deltaTime * m_wanderLerpQuickness * .5f);
        }

        private void Chase() {
            var pos = transform.position;
            var targetPos = PlayerController.Singleton.transform.position;

            _moveDirection = (targetPos - pos).normalized;

            if (Physics.Raycast(pos, _moveDirection, out var hit, m_maxWallCastDistance)) {
                if (!hit.collider.CompareTag("Player"))
                    _moveDirection = Quaternion.Euler(transform.right * -90f) * hit.normal; // Bend upwards
            }

            _body.velocity = Vector3.Slerp(_body.velocity, _moveDirection * m_chaseSpeed, Time.deltaTime * m_chaseLerpQuickness);
            transform.forward = Vector3.Slerp(transform.forward, _moveDirection, Time.deltaTime * m_chaseLerpQuickness * .5f);
        }

        private void Flee() => _body.velocity = _moveDirection * m_fleeSpeed;

        private void CheckDistancePeriodically() {
            var pos = transform.position;
            var playerPos = PlayerController.Singleton.transform.position;
            var distFromPlayer = Vector3.Distance(pos, playerPos);

            var verticalDistance = Mathf.Abs(pos.y - playerPos.y);

            if (distFromPlayer < m_detectionDistance) {
                SetState(WakakaState.Chase);
                _maskAnimator.AnimateLaughter();
                return;
            }

            if (distFromPlayer < m_maxDistanceFromPlayer && verticalDistance < m_maxDistanceYFromPlayer)
                return;

            // _moveDirection = -_moveDirection;
            _moveDirection = (PlayerController.Singleton.transform.position - transform.position).normalized;
        }

        private void DoRayCastAndAdjustDirection(ref Vector3 direction) {
            if (!Physics.SphereCast(transform.position, .25f, direction, out var hit, m_maxWallCastDistance))
                return;

            direction = hit.normal;
        }
        #endregion

        #region Event Methods
        private void ForceChasePlayer() {
            OnEveryoneChasePlayer -= ForceChasePlayer;
            SetState(WakakaState.Chase);
        }

        private void OnDie() {
            OnEveryoneChasePlayer -= ForceChasePlayer;

            OnEveryoneChasePlayer?.Invoke();
            Destroy(_collider);
            Destroy(_attacker.gameObject);
            SetState(WakakaState.Flee);

            _maskAnimator.AnimateDeath();
            OnDeath?.Invoke(this);
        }
        #endregion
    }
}