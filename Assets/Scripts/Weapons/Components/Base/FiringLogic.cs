using Code.Weapons;
using UnityEngine;

namespace Code.Weapon {

    public abstract class FiringLogic : MonoBehaviour {
        [Header("Settings")]
        [SerializeField] protected float range = default;
        [SerializeField] protected float cooldown = default;

        protected float elapsedTime = default;
        protected bool cooldownActive = default;

        protected virtual void Update() {
            if (!cooldownActive)
                return;

            elapsedTime += Time.deltaTime;

            if(elapsedTime >= cooldown) {
                elapsedTime = 0f;
                cooldownActive = false;
            }
        }

        public virtual bool CanShoot() {
            return !cooldownActive;
        }

        public abstract void Shoot(Ammunition ammunition);
    }

}