using Code.Player;
using Code.Weapons;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Code.UI
{
    public class WeaponBehaviourUI : MonoBehaviour
    {
        #region Public Variables
        [SerializeField] private Image m_selectedWeapon;
        [SerializeField] private TMP_Text m_munitions;
        #endregion

        #region Properties
        [SerializeField] private List<Sprite> weaponSprites;
        #endregion

        #region Private Variables
        #endregion

        #region Behaviour Callbacks
        private void Start()
        {
            if(!PlayerController.Singleton)
                return;

            PlayerController.Singleton.OnWeaponChanged += UpdateWeaponIcon;
            PlayerController.Singleton.gameObject.GetComponent<PlayerWeaponHandler>().OnUpdateWeaponInfo += CheckWeapon;
        }
        private void OnDestroy()
        {
            if (PlayerController.Singleton)
                PlayerController.Singleton.OnWeaponChanged -= UpdateWeaponIcon;
        }
        #endregion

        #region Public Methods
        public void UpdateWeaponIcon(int i)
        {
            m_selectedWeapon.sprite = weaponSprites[i];
        }

        private void CheckWeapon(Weapon weapon) => m_munitions.text = $"{weapon.Cartridge.CurrentAmount}/{weapon.Cartridge.TotalAmount}";
        #endregion

        #region Private Methods
        #endregion

        #region Virtual Methods
        #endregion
    }
}
