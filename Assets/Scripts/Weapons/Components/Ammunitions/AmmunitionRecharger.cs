using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Weapons {
    public class AmmunitionRecharger : MonoBehaviour, IRecharger {
        #region Public Variables
        [Header("Settings")]
        [field: SerializeField] public WeaponType Type { get; private set; }
        [field: SerializeField] public int Amount { get; private set; }
        [SerializeField] private float cooldown;

        [Header("References")]
        [SerializeField] private Collider interactableCollider;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private GameObject cooldownCanvas;
        [SerializeField] private Image cooldownFiller;
        #endregion

        #region Private Variables
        private float _elapsedTime;
        private bool _isRecharging;
        #endregion

        #region Behaviour Callbacks
        private void Update() {
            if (!_isRecharging)
                return;

            if(!cooldownCanvas.activeSelf)
                cooldownCanvas.SetActive(true);

            _elapsedTime += Time.deltaTime;
            cooldownFiller.fillAmount = _elapsedTime/cooldown;

            if (_elapsedTime < cooldown)
                return;

            _elapsedTime = 0;

            if (cooldownCanvas.activeSelf)
                cooldownCanvas.SetActive(false);

            SetInteractable(true);
        }
        #endregion

        #region Public Methods
        public void SetInteractable(bool state) {
            _isRecharging = !state;

            interactableCollider.enabled = state;
            meshRenderer.enabled = state;
        }
        #endregion
    }
}