using UnityEngine;

namespace Code.Weapons {

    public class AmmunitionRecharger : MonoBehaviour, IRecharger {
        [Header("Settings")]
        [SerializeField] private WeaponType type = default;
        [SerializeField] private int amount = default;

        public WeaponType GetCompatibleWeapon() {
            return type;
        }
        public int GetReloadAmount() {
            return amount;
        }
    }

}