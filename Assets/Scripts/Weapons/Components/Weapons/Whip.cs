using LanguageSystem.Runtime.Utility;
using UnityEngine;

namespace Code.Weapons {
    public class Whip : Weapon {
        #region Public Variables
        [field: SerializeReference] public override FiringLogicBase FiringLogic { get; protected set; } = new PhysicHitLogic();
        [SerializeField] private float m_cooldownDuration = .75f;
        [SerializeField] private LocalizedString m_chargedText;
        #endregion

        #region Private Variables
        private bool _isInCooldown;
        #endregion

        #region Properties
        private readonly WeaponCooldownChargeStatus _chargeStatus = new();
        public override WeaponChargeStatus ChargeStatus => _chargeStatus;
        #endregion

        #region Overrides
        protected override void OnStart() => ClearCooldown();

        protected override void OnShoot() {
            _isInCooldown = true;

            _chargeStatus.Info = "...";
            _chargeStatus.CooldownProgress = 0f;
            _chargeStatus.Dispatch();

            Invoke(nameof(ClearCooldown), m_cooldownDuration);
        }

        public override bool CanShoot() => !_isInCooldown;

        public override bool Recharge(int amount) => true;
        #endregion

        private void ClearCooldown() {
            _isInCooldown = false;
            _chargeStatus.Info = m_chargedText.GetLocalizedString();
            _chargeStatus.CooldownProgress = 1f;
            _chargeStatus.Dispatch();
        }
    }
}