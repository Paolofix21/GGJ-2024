using Code.Player;
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

        protected PlayerWeaponHandler handler;

        public WeaponType WeaponType { get { return weaponType; } }
        public Cartridge Cartridge { get { return cartridge; } }
        public FiringLogic Logic { get { return firingLogic; } }

        public virtual bool CanShoot() {
            return firingLogic.CanShoot() && cartridge.HasAmmo();
        }

        public virtual void Shoot() {
            firingLogic.Shoot(ammunition);
            cartridge.Consume();
        }

        public virtual void Recharge(int ammount) {
            cartridge.AddAmmo(ammount);
        }

        public void Boost() => firingLogic.Boost();

        public void SetUp(PlayerWeaponHandler playerWeaponHandler) {
            handler = playerWeaponHandler;
        }
    }

}