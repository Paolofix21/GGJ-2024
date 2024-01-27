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
        private int _maxAmmo;
        private int _currentAmmo;
        private Cartridge _cartridgeSelected;
        #endregion

        #region Behaviour Callbacks
        private void Start()
        {
            if(PlayerController.Singleton)
                PlayerController.Singleton.OnWeaponChanged += UpdateWeaponIcon;
        }
        private void OnDestroy()
        {
            if (PlayerController.Singleton)
                PlayerController.Singleton.OnWeaponChanged -= UpdateWeaponIcon;
        }
        private void Update()
        {
            if (_cartridgeSelected && _currentAmmo != _cartridgeSelected.CurrentAmount)
            {
                m_munitions.text = $"{_currentAmmo}/{_maxAmmo}";
            }
        }
        #endregion

        #region Public Methods
        public void UpdateWeaponIcon(int i)
        {
            m_selectedWeapon.sprite = weaponSprites[i];
            _cartridgeSelected = PlayerController.Singleton.gameObject.GetComponent<PlayerWeaponHandler>().EquippedWeapon.Cartridge;
            _maxAmmo = _cartridgeSelected.TotalAmount;
        }
        #endregion

        #region Private Methods
        #endregion

        #region Virtual Methods
        #endregion
    }
}
