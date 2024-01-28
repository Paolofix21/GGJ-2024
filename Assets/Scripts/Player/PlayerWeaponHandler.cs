using Code.Player;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.Weapons {
    public class PlayerWeaponHandler : MonoBehaviour {
        [Header("Settings")]
        [SerializeField] private WeaponType defaultType = default;

        [Header("References")]
        [SerializeField] private List<Weapon> weapons = new List<Weapon>();
        [Space]
        [SerializeField] private PlayerController playerController = default;
        [SerializeField] private PlayerWeaponAnimatorListener playerWeaponAnimatorListener = default;

        private Weapon equippedWeapon = default;
        public Weapon EquippedWeapon { get { return equippedWeapon; } }

        public event System.Action<Weapon> OnUpdateWeaponInfo;

        private void OnValidate() {
            if (playerWeaponAnimatorListener == null) {
                playerWeaponAnimatorListener = GetComponentInChildren<PlayerWeaponAnimatorListener>();
            }
        }
        private void Awake() {
            if (playerController != null) {
                playerController.OnWeaponChanged += EquipWeapon;
                playerController.OnShootRequest += CanShoot;
            }
            if(playerWeaponAnimatorListener != null) {
                playerWeaponAnimatorListener.OnAnimatorShootCallback += Shoot;
            }
        }
        private void Start() {
            EquipWeapon(defaultType);
        }
        private void OnDestroy() {
            if (playerController != null) {
                playerController.OnWeaponChanged -= EquipWeapon;
                playerController.OnShootRequest -= CanShoot;
            }
            if (playerWeaponAnimatorListener != null) {
                playerWeaponAnimatorListener.OnAnimatorShootCallback -= Shoot;
            }
        }
        private void OnTriggerEnter(Collider other) {
            if (!other.gameObject.CompareTag("Ammunition"))
                return;

            IRecharger recharger = other.GetComponent<IRecharger>();
            InteractWithRecharger(recharger);
        }

        private void EquipWeapon(WeaponType weaponType) => EquipWeapon((int)weaponType);

        private void EquipWeapon(int weaponType) {
            if (equippedWeapon)
                equippedWeapon.Cartridge.OnAmmoAmountChanged -= CheckAmmo;

            equippedWeapon = weapons.First(weapon => weapon.WeaponType == (WeaponType)weaponType);
            OnUpdateWeaponInfo?.Invoke(equippedWeapon);

            if (equippedWeapon)
                equippedWeapon.Cartridge.OnAmmoAmountChanged += CheckAmmo;

            playerController.visualSetter.SetEmissivePower(equippedWeapon.Cartridge.GetAmmoRate());
        }

        private void CheckAmmo(int ammoCount) {
            OnUpdateWeaponInfo?.Invoke(equippedWeapon);
            playerController.visualSetter.SetEmissivePower(equippedWeapon.Cartridge.GetAmmoRate());
        }

        private bool CanShoot() {
            return equippedWeapon.CanShoot();
        }

        [UsedImplicitly]
        private void Shoot() {
            equippedWeapon.Shoot();
        }

        private void InteractWithRecharger(IRecharger recharger) {
            if (recharger == null) return;

            RechargeWeapon(recharger.GetCompatibleWeapon(), recharger.GetReloadAmount());
            recharger.Interactable(false);
        }

        private void RechargeWeapon(WeaponType type, int amount) {
            Weapon weaponToRecharge = weapons.First(weapon => weapon.WeaponType == type);
            weaponToRecharge.Recharge(amount);
        }
    }
}