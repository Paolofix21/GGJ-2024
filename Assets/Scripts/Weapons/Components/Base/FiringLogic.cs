using FMODUnity;
using System;
using UnityEngine;

namespace Code.Weapons {

    public abstract class FiringLogic : MonoBehaviour {
        [Header("Base")]
        [SerializeField] protected float range = default;
        [SerializeField] protected float cooldown = default;

        [Header("References")]
        [SerializeField] protected EventReference soundEventReference = default;
        [Space]
        [SerializeField] protected Transform weaponCamera = default;
        [SerializeField] protected Transform effectOrigin = default;

        protected float elapsedTime = default;
        protected bool cooldownActive = default;

        public Action<bool> OnCooldownStateChanged = default;

        protected virtual void Update() {
            if (!cooldownActive)
                return;

            elapsedTime += Time.deltaTime;

            if(elapsedTime >= cooldown) {
                elapsedTime = 0f;
                Cooldown(false);
            }
        }

        public virtual bool CanShoot() {
            return !cooldownActive;
        }

        protected virtual void Cooldown(bool state) {
            cooldownActive = state;
            OnCooldownStateChanged?.Invoke(state);
        }

        public abstract void Shoot(Ammunition ammunition);

        protected abstract void Effect(Vector3 position);
    }

}