using UnityEngine;

namespace Code.Weapons {
    public class Shotgun : Weapon {
        #region Public Variables
        [field: SerializeField] public Cartridge Cartridge { get; private set; }
        [field: SerializeReference] public override FiringLogicBase FiringLogic { get; protected set; } = new ZoneBulletLogic();
        [SerializeField] private float m_cooldownDuration = .5f;
        #endregion

        #region Private Variables
        private bool _isInCooldown;
        #endregion

        #region Properties
        private readonly WeaponBulletChargeStatus _chargeStatus = new();
        public override WeaponChargeStatus ChargeStatus => _chargeStatus;
        #endregion

        #region Behaviour Callbacks
        protected override void OnStart() {
            Cartridge.Init();
            Cartridge.OnAmmoAmountChanged += Refresh;
            Refresh(Cartridge.CurrentAmount);
        }
        #endregion

        #region Public Methods
        public override bool CanShoot()
        {
            if (_isInCooldown)
                return false;

            if (Cartridge.CurrentAmount > 0)
                return true;

            CantShoot();
            return false;
        }
        protected override void OnShoot() {
            _isInCooldown = true;

            Cartridge.Consume();
            Refresh(Cartridge.CurrentAmount);

            Invoke(nameof(ClearCooldown), m_cooldownDuration);
        }

        public override bool Recharge(int amount) {
            if (Cartridge.IsFull())
                return false;

            Cartridge.AddAmmo(amount);
            return true;
        }
        #endregion

        #region Event Methods
        private void ClearCooldown() {
            _isInCooldown = false;
            Refresh(0);
        }

        private void Refresh(int _) {
            _chargeStatus.Info = $"{Cartridge.CurrentAmount}/{Cartridge.TotalAmount}";
            _chargeStatus.CurrentBullets = Cartridge.CurrentAmount;
            _chargeStatus.MaxBullets = Cartridge.TotalAmount;
            _chargeStatus.Dispatch();
        }
        #endregion

    }
}