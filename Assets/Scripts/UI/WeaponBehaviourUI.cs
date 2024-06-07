using Code.Player;
using Code.Weapons;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Code.Core.MatchManagers;
using Code.Promises;

namespace Code.UI {
    public class WeaponBehaviourUI : MonoBehaviour {
        #region Public Variables
        [SerializeField] private Image m_selectedWeapon;
        [SerializeField] private TMP_Text m_munitions;
        [SerializeField] private float m_highlightSize = 1.4f;

        [Header("Special Weapon")]
        [SerializeField] private Image m_specialWeaponFiller;
        [SerializeField] private Image m_specialWeaponIconImage;
        [SerializeField] private Sprite m_specialWeaponFilledSprite;
        #endregion

        #region Properties
        [SerializeField] private List<Sprite> weaponSprites;
        [SerializeField] private List<Image> weaponImages;
        #endregion

        #region Private Variables
        private PlayerController _target;
        private Sword _sword;
        #endregion

        #region Behaviour Callbacks
        private IEnumerator Start() {
            yield return null;
            _target = GameEvents.MatchManager.GetPlayerEntity().Transform.GetComponent<PlayerController>();
            _sword = _target.GetComponentInChildren<Sword>();
            _target.OnWeaponChanged += UpdateWeaponIcon;
            var playerWeaponHandler = _target.gameObject.GetComponent<PlayerWeaponHandler>();
            playerWeaponHandler.OnUpdateWeaponInfo += CheckWeapon;
            GameEvents.GetMatchManager<WaveBasedMatchManager>().EntityManager.Entities.OnRemoved += DisplayWeaponEnergy;
            _sword.Shot += DisplayWeaponEnergy;
            CheckWeapon(playerWeaponHandler.EquippedWeapon);
            UpdateWeaponIcon((int)playerWeaponHandler.EquippedWeapon.WeaponType);
            DisplayWeaponEnergy();
        }

        private void DisplayWeaponEnergy(IEntity element) => DisplayWeaponEnergy();

        private void OnDestroy() {
            if (!_target)
                return;

            _target.OnWeaponChanged -= UpdateWeaponIcon;
            _target.gameObject.GetComponent<PlayerWeaponHandler>().OnUpdateWeaponInfo -= CheckWeapon;
            var mm = GameEvents.GetMatchManager<WaveBasedMatchManager>();
            if (mm)
                mm.EntityManager.Entities.OnRemoved -= DisplayWeaponEnergy;
            _sword.Shot -= DisplayWeaponEnergy;
        }
        #endregion

        #region Public Methods
        public void UpdateWeaponIcon(int i) {
            m_selectedWeapon.sprite = weaponSprites[i];
            for (var j = 0; j < weaponImages.Count; j++)
                weaponImages[j].transform.localScale = Vector3.one * (i == j ? m_highlightSize : 1f);
        }

        private void CheckWeapon(Weapon weapon) => m_munitions.text = $"{weapon?.ChargeStatus.Info}";
        #endregion

        #region Private Methods
        private async void DisplayWeaponEnergy() {
            await Task.Yield();
            if (!this) // Safeguard for errors
                return;

            m_specialWeaponFiller.fillAmount = ((WeaponEnergyChargeStatus)_sword.ChargeStatus).EnergyAmount;
            m_specialWeaponIconImage.overrideSprite = m_specialWeaponFiller.fillAmount >= .99f ? m_specialWeaponFilledSprite : null;
        }
        #endregion

        #region Virtual Methods
        #endregion
    }
}