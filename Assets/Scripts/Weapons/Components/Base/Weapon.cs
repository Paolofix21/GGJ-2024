using UnityEngine;

namespace Code.Weapons {
    public abstract class Weapon : MonoBehaviour {
        #region Public Variables
        [field: Header("Settings")]
        [field: SerializeField] public WeaponType WeaponType { get; private set; }
        [field: SerializeField] public Ammunition Ammunition { get; private set; }
        public abstract FiringLogicBase FiringLogic { get; protected set; }

        public event System.Action OnCantShoot;
        public event System.Action Shot;
        #endregion

        #region Private Variables
        protected PlayerWeaponHandler _handler;
        #endregion

        #region Properties
        public abstract WeaponChargeStatus ChargeStatus { get; }
        #endregion

        #region Behaviour Callbacks
        private void Start() {
            FiringLogic.Init(this);
            OnStart();
            ChargeStatus.Dispatch();
        }
        #endregion

        #region Public Methods
        public void Init(PlayerWeaponHandler playerWeaponHandler) => _handler = playerWeaponHandler;

        public abstract bool CanShoot();

        public void Shoot() {
            Shot?.Invoke();
            OnShoot();
            FiringLogic.Shoot(Ammunition);
        }

        public abstract bool Recharge(int amount);

        public void Boost() => FiringLogic.Boost();

        public virtual void SetSide(bool right) { }
        #endregion

        #region Protected Methods
        protected virtual void OnStart() { }

        protected virtual void OnShoot() {}

        protected void CantShoot() => OnCantShoot?.Invoke();
        #endregion
    }
}