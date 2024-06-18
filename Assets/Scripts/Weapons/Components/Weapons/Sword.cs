using Code.Core;
using Code.Promises;
using LanguageSystem.Runtime.Utility;
using UnityEngine;

namespace Code.Weapons {
    public class Sword : Weapon {
        #region Public Variables
        [field: SerializeReference] public override FiringLogicBase FiringLogic { get; protected set; } = new PhysicHitLogic();
        [SerializeField] private int m_maxEnergy = 10;
        [SerializeField] private LocalizedString m_chargedText, m_dischargedText;
        #endregion

        #region Private Variables
        private int _currentEnergy = 10;
        #endregion

        #region Properties
        private readonly WeaponEnergyChargeStatus _chargeStatus = new();
        public override WeaponChargeStatus ChargeStatus => _chargeStatus;

        public int CurrentEnergy => _currentEnergy;
        #endregion

        #region Behaviour Callbacks
        private void Awake() => _currentEnergy = m_maxEnergy;

        protected override void OnStart() {
            FindFirstObjectByType<EntityManager>().Entities.OnRemoved += EnemyRemoved;
            Refresh();
        }
        #endregion

        #region Overrides
        public override bool CanShoot() {
            if (_currentEnergy >= m_maxEnergy)
                return true;

            CantShoot();
            return false;
        }

        protected override void OnShoot() {
            _currentEnergy = 0;
            Refresh();
        }

        public override bool Recharge(int amount) => true;
        #endregion

        #region Event Methods
        private void EnemyRemoved(IEntity element) {
            ++_currentEnergy;
            Refresh();
        }

        private void Refresh() {
            _chargeStatus.Info = CanShoot() ? m_chargedText.GetLocalizedString() : m_dischargedText.GetLocalizedString();
            _chargeStatus.EnergyAmount = (float)_currentEnergy / m_maxEnergy;
            _chargeStatus.Dispatch();
        }
        #endregion
    }
}