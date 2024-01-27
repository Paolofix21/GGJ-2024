using UnityEngine;

namespace Code.Weapons {

    [RequireComponent(typeof(Cartridge))]
    public abstract class Weapon : MonoBehaviour {
        [Header("Settings")]
        [SerializeField] protected WeaponType weaponType = default;
        [Space]
        [SerializeField] protected Ammunition ammunition = default;

        [Header("References")]
        [SerializeField] protected Cartridge cartridge = default;
        [SerializeField] protected FiringLogic firingLogic = default;

        protected virtual void Shoot() {
            if (!firingLogic.CanShoot())
                return;

            if (!cartridge.HasAmmo())
                return;

            cartridge.Consume();
            firingLogic.Shoot(ammunition);
        }
    }

}