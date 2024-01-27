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

        public WeaponType WeaponType { get { return weaponType; } }
        public Cartridge Cartridge { get { return cartridge; } }
        public FiringLogic Logic { get { return firingLogic; } }

        public virtual bool Shoot() {
            if (!firingLogic.CanShoot())
                return false;

            if (!cartridge.HasAmmo())
                return false;

            firingLogic.Shoot(ammunition);
            cartridge.Consume();
            return true;
        }

        public virtual void Recharge(int ammount) {
            cartridge.AddAmmo(ammount);
        }
    }

}