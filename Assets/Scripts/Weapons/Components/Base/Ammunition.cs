using UnityEngine;

namespace Code.Weapons {
    [System.Serializable] 
    public struct Ammunition {
        [field: SerializeField] public DamageType DamageType { get; private set; }
        [field: SerializeField] public float DamageAmount { get; private set; }
    }
}