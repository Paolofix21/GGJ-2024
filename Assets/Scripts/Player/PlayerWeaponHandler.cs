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

        private Weapon equippedWeapon = default;
        public Weapon EquippedWeapon { get { return equippedWeapon; } }

        private void Awake() {
            if (playerController != null) {
                playerController.OnWeaponChanged += EquipWeapon;
                playerController.OnShootRequest += CanShoot;
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
        }
        private void OnTriggerEnter(Collider other) {
            if (!other.gameObject.CompareTag("Ammunition"))
                return;

            IRecharger recharger = other.GetComponent<IRecharger>();
            InteractWithRecharger(recharger);
        }

        private void EquipWeapon(WeaponType weaponType) {
            equippedWeapon = weapons.First(weapon => weapon.WeaponType == weaponType);
        }
        private void EquipWeapon(int weaponType) {
            WeaponType type = (WeaponType)weaponType;
            equippedWeapon = weapons.First(weapon => weapon.WeaponType == type);
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