using UnityEngine;

namespace Code.Weapons {
    public class Pistol : Weapon {
        #region Public Variables
        [field: SerializeField] public Cartridge Cartridge { get; private set; }
        [field: SerializeReference] public override FiringLogicBase FiringLogic { get; protected set; } = new SingleBulletLogic();
        [SerializeField] private float m_cooldownDuration = .5f;
        #endregion

        #region Properties
        private readonly WeaponBulletChargeStatus _chargeStatus = new();
        public override WeaponChargeStatus ChargeStatus => _chargeStatus;
        #endregion

        #region Private Variables
        private bool _isInCooldown;
        #endregion

        #region Behaviour Callbacks
        protected override void OnStart() {
            Cartridge.Init();
            Cartridge.OnAmmoAmountChanged += Refresh;
            Refresh(Cartridge.CurrentAmount);
        }
        #endregion

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

        public override void Recharge(int amount) => Cartridge.AddAmmo(amount);

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