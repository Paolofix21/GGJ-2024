using Code.Player;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;

namespace Code.Weapons {
    public class PlayerWeaponHandler : MonoBehaviour {
        #region Public Variables
        [Header("Settings")]
        [SerializeField] private WeaponType defaultType;
        [SerializeField] private EventReference m_rechargeSound;

        [Header("References")]
        [SerializeField] private List<Weapon> weapons = new();

        [Space]
        [SerializeField] public PlayerController playerController;
        [SerializeField] private PlayerWeaponAnimatorListener playerWeaponAnimatorListener;

        public event System.Action<Weapon> OnUpdateWeaponInfo;
        #endregion

        #region Properties
        public Weapon EquippedWeapon { get; private set; }
        #endregion

        #region Behaviour Callbacks
        private void OnValidate() {
            if (playerWeaponAnimatorListener == null)
                playerWeaponAnimatorListener = GetComponentInChildren<PlayerWeaponAnimatorListener>();
        }

        private void Awake() {
            if (playerController != null) {
                playerController.OnWeaponChanged += EquipWeapon;
                playerController.OnShootRequest += CanShoot;
            }

            if(playerWeaponAnimatorListener != null)
                playerWeaponAnimatorListener.OnAnimatorShootCallback += Shoot;

            weapons.ForEach(w => w.Init(this));
        }

        private IEnumerator Start() {
            yield return null;
            EquipWeapon(defaultType);
        }

        private void OnTriggerEnter(Collider other) {
            if (other.TryGetComponent(out IRecharger recharger))
                InteractWithRecharger(recharger);
        }

        private void OnDestroy() {
            if (playerController != null) {
                playerController.OnWeaponChanged -= EquipWeapon;
                playerController.OnShootRequest -= CanShoot;
            }

            if (playerWeaponAnimatorListener != null)
                playerWeaponAnimatorListener.OnAnimatorShootCallback -= Shoot;
        }
        #endregion

        #region Public Methods
        [UsedImplicitly]
        public void BoostAllWeapons() => weapons.ForEach(w => w.Boost());
        #endregion

        #region Private Methods
        [UsedImplicitly]
        private void Shoot() => EquippedWeapon.Shoot();

        private void InteractWithRecharger(IRecharger recharger) {
            if (recharger == null) return;

            RechargeWeapon(recharger.Type, recharger.Amount);
            recharger.SetInteractable(false);
            RuntimeManager.PlayOneShotAttached(m_rechargeSound, gameObject);
        }

        private void RechargeWeapon(WeaponType type, int amount) {
            var weaponToRecharge = weapons.Find(weapon => weapon.WeaponType == type);
            weaponToRecharge.Recharge(amount);
        }
        #endregion

        #region Event Methods
        private void EquipWeapon(WeaponType weaponType) => EquipWeapon((int)weaponType);

        private void EquipWeapon(int weaponType) {
            if (EquippedWeapon)
                EquippedWeapon.ChargeStatus.OnUpdated -= CheckAmmo;

            EquippedWeapon = weapons.First(weapon => weapon.WeaponType == (WeaponType)weaponType);
            OnUpdateWeaponInfo?.Invoke(EquippedWeapon);

            if (EquippedWeapon)
                EquippedWeapon.ChargeStatus.OnUpdated += CheckAmmo;

            SetEmissionChargeFeedback(EquippedWeapon.ChargeStatus);
        }

        private void CheckAmmo(WeaponChargeStatus weaponChargeStatus) {
            OnUpdateWeaponInfo?.Invoke(EquippedWeapon);
            SetEmissionChargeFeedback(weaponChargeStatus);
        }

        private void SetEmissionChargeFeedback(WeaponChargeStatus weaponChargeStatus) {
            switch (weaponChargeStatus) {
                case WeaponEnergyChargeStatus energyWeapon:
                    playerController.visualSetter.SetEmissivePower(Mathf.Clamp01(energyWeapon.EnergyAmount));
                    return;
                case WeaponCooldownChargeStatus cooldownWeapon:
                    playerController.visualSetter.SetEmissivePower(Mathf.Clamp01(cooldownWeapon.CooldownProgress));
                    return;
                case WeaponBulletChargeStatus bulletWeapon:
                    playerController.visualSetter.SetEmissivePower(Mathf.Clamp01((float)bulletWeapon.CurrentBullets / bulletWeapon.MaxBullets));
                    return;
            }
        }

        private bool CanShoot() => EquippedWeapon && EquippedWeapon.CanShoot();
        #endregion
    }
}