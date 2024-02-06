using UnityEngine;

namespace Code.Weapons {
    public class Shotgun : Weapon {
        [field: SerializeField] public Cartridge Cartridge { get; private set; }
        [field: SerializeReference] public override FiringLogicBase FiringLogic { get; protected set; } = new ZoneBulletLogic();

        private readonly WeaponBulletChargeStatus _chargeStatus = new();
        public override WeaponChargeStatus ChargeStatus => _chargeStatus;

        #region Behaviour Callbacks
        protected override void OnStart() {
            Cartridge.Init();
            Cartridge.OnAmmoAmountChanged += Refresh;
            Refresh(Cartridge.CurrentAmount);
        }
        #endregion

        public override bool CanShoot() => Cartridge.CurrentAmount > 0;

        protected override void OnShoot() {
            Cartridge.Consume();
            Refresh(Cartridge.CurrentAmount);
        }

        public override void Recharge(int amount) => Cartridge.AddAmmo(amount);

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