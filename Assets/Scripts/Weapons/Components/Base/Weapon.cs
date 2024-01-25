using Code.Weapon;
using UnityEngine;

namespace Code.Weapons {

    [RequireComponent(typeof(Cartridge))]
    public class Weapon : MonoBehaviour {
        [Header("Settings")]
        [SerializeField] private WeaponType weaponType = default;
        [Space]
        [SerializeField] private Ammunition ammunition = default;

        [Header("References")]
        [SerializeField] private Cartridge cartridge = default;
        [SerializeField] private FiringLogic firingLogic = default;

        private void OnValidate() {
            if(cartridge == null) 
                cartridge = GetComponent<Cartridge>();
        }
    }

}