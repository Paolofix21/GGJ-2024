using UnityEngine;

namespace Code.Weapons {
    public class AutomaticRifle : Weapon {
        #region Public Variables
        [field: SerializeField] public Cartridge Cartridge { get; private set; }
        [field: SerializeReference] public override FiringLogicBase FiringLogic { get; protected set; } = new SingleBulletLogic();
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

        #region Overrides
        public override bool CanShoot() => Cartridge.CurrentAmount > 0;

        protected override void OnShoot() {
            if (Cartridge.CurrentAmount <= 0)
                _handler.playerController.PlayShootContinuous(false);

            Cartridge.Consume();
            Refresh(Cartridge.CurrentAmount);
        }

        public override void Recharge(int amount) => Cartridge.AddAmmo(amount);
        #endregion

        #region Event Methods
        private void Refresh(int ammoCount) {
            _chargeStatus.Info = $"{Cartridge.CurrentAmount}/{Cartridge.TotalAmount}";
            _chargeStatus.CurrentBullets = Cartridge.CurrentAmount;
            _chargeStatus.MaxBullets = Cartridge.TotalAmount;
            _chargeStatus.Dispatch();
        }
        #endregion
    }
}