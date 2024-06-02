using Code.EnemySystem;
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
using System;
namespace Code.UI
{
    public class WeaponBehaviourUI : MonoBehaviour
    {
        #region Public Variables
        [SerializeField] private Image m_selectedWeapon;
        [SerializeField] private TMP_Text m_munitions;
        [SerializeField] private Image m_specialWeaponFiller;
        #endregion

        #region Properties
        [SerializeField] private List<Sprite> weaponSprites;
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
            _target.gameObject.GetComponent<PlayerWeaponHandler>().OnUpdateWeaponInfo += CheckWeapon;
            GameEvents.GetMatchManager<WaveBasedMatchManager>().EntityManager.Entities.OnRemoved += DisplayWeaponEnergy;
            _sword.Shot += DisplayWeaponEnergy;
            CheckWeapon(_target.gameObject.GetComponent<PlayerWeaponHandler>().EquippedWeapon);
        }

        private void DisplayWeaponEnergy(IEntity element)
        {
            DisplayWeaponEnergy();
        }

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
        public void UpdateWeaponIcon(int i)
        {
            m_selectedWeapon.sprite = weaponSprites[i];
        }

        private void CheckWeapon(Weapon weapon) => m_munitions.text = $"{weapon?.ChargeStatus.Info}";
        #endregion

        #region Private Methods
        private async void DisplayWeaponEnergy() {
            await Task.Yield();
            if (!this) // Safeguard for errors
                return;
            m_specialWeaponFiller.fillAmount = _sword.CurrentEnergy / 20.0f;
        }
        #endregion

        #region Virtual Methods
        #endregion
    }
}
