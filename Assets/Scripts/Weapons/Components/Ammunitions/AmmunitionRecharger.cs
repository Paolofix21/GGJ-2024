using Code.Player;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Weapons {

    public class AmmunitionRecharger : MonoBehaviour, IRecharger {
        [Header("Settings")]
        [SerializeField] private WeaponType type = default;
        [SerializeField] private int amount = default;
        [SerializeField] private float cooldown = default;

        [Header("References")]
        [SerializeField] private Collider interactableCollider = default;
        [SerializeField] private MeshRenderer meshRenderer = default;
        [SerializeField] private GameObject cooldownCanvas;
        [SerializeField] private Image cooldownFiller;
        [SerializeField] private EventReference reload;
        private float elapsedTime = default;
        private bool isRecharging = default; 

        private void Update() {
            if (!isRecharging)
                return;
            if(!cooldownCanvas.activeSelf)
                cooldownCanvas.SetActive(true);
            elapsedTime += Time.deltaTime;
            cooldownFiller.fillAmount = elapsedTime/cooldown;
            if (elapsedTime < cooldown)
                return;

            elapsedTime = 0;
            if (cooldownCanvas.activeSelf)
                cooldownCanvas.SetActive(false);
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
            if(!state)
            {
                AudioManager.instance.PlayOneShot(reload);
            }
        }

    }

}