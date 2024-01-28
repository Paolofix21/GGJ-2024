using UnityEngine;

namespace Code.Weapons {

    public class AmmunitionRecharger : MonoBehaviour, IRecharger {
        [Header("Settings")]
        [SerializeField] private WeaponType type = default;
        [SerializeField] private int amount = default;
        [SerializeField] private float cooldown = default;

        [Header("References")]
        [SerializeField] private Collider interactableCollider = default;
        [SerializeField] private MeshRenderer meshRenderer = default;

        private float elapsedTime = default;
        private bool isRecharging = default; 

        private void Update() {
            if (!isRecharging)
                return;

            elapsedTime += Time.deltaTime;
            if (elapsedTime < cooldown)
                return;

            elapsedTime = 0;
            Interactable(true);
        }

        public WeaponType GetCompatibleWeapon() {
            return type;
        }
        public int GetReloadAmount() {
            return amount;
        }

        public void Interactable(bool state) {
            isRecharging = !state;

            interactableCollider.enabled = state;
            meshRenderer.enabled = state;
        }

    }

}