using Code.Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.Weapons {
    public class PlayerWeaponHandler : MonoBehaviour {
        [Header("Settings")]
        [SerializeField] private WeaponType defaultWeapon = default;

        [Header("References")]
        [SerializeField] private List<Weapon> weapons = new List<Weapon>();
        [Space]
        [SerializeField] private PlayerController playerController = default;

        private Weapon equippedWeapon = default;

        private void Awake() {
            if (playerController != null) {
                playerController.OnWeaponChanged += EquipWeapon;
                playerController.OnShootRequest += ShotRequest;
            }
        }
        private void OnDestroy() {
            if (playerController != null) {
                playerController.OnWeaponChanged -= EquipWeapon;
                playerController.OnShootRequest -= ShotRequest;
            }
        }
        private void OnTriggerEnter(Collider other) {
            if (!other.gameObject.CompareTag("Ammunitions"))
                return;

            IRecharger recharger = other.GetComponent<IRecharger>();
            InteractWithRecharger(recharger);
        }

        private void EquipWeapon(int weaponType) {
            WeaponType type = (WeaponType)weaponType;
            equippedWeapon = weapons.First(weapon => weapon.WeaponType == type);
        }

        private bool ShotRequest() {
            return equippedWeapon.Shoot();
        }

        private void InteractWithRecharger(IRecharger recharger) {
            if (recharger == null) return;

            RechargeWeapon(recharger.GetCompatibleWeapon(), recharger.GetReloadAmount());
        }

        private void RechargeWeapon(WeaponType type, int amount) {
            Weapon weaponToRecharge = weapons.First(weapon => weapon.WeaponType == type);
            weaponToRecharge.Recharge(amount);
        }
    }
}