using UnityEngine;

namespace Code.Weapons {

    public interface IDamageable {
        public bool GetDamage(DamageType damageType);
        public void ApplyDamage(float amount, GameObject dealer);
    }

}