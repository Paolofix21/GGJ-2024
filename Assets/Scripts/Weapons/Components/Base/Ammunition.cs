using UnityEngine;

namespace Code.Weapons {

    [System.Serializable] 
    public struct Ammunition {
        [SerializeField] private DamageType damageType;
        [SerializeField] private float damageAmount;

        public DamageType GetDamageType() { return damageType; }
        public float GetDamageAmount() {  return damageAmount; }
    }

}