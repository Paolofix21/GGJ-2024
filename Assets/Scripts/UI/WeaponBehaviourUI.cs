using Code.EnemySystem;
using Code.Player;
using Code.Weapons;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        [SerializeField] private Image m_specialWeaponFiller;
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
            WaveSpawner.OnEnemyDeath += DisplayWeaponEnergy;
            Sword.OnShoot += DisplayWeaponEnergy;
        }
        private void OnDestroy()
        {
            if (PlayerController.Singleton)
            {
                PlayerController.Singleton.OnWeaponChanged -= UpdateWeaponIcon;
                PlayerController.Singleton.gameObject.GetComponent<PlayerWeaponHandler>().OnUpdateWeaponInfo -= CheckWeapon;
            }
            WaveSpawner.OnEnemyDeath -= DisplayWeaponEnergy;
            Sword.OnShoot -= DisplayWeaponEnergy;
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
        private async void DisplayWeaponEnergy()
        {
            await Task.Yield();
            m_specialWeaponFiller.fillAmount = Sword.currentEnergy / 10.0f;
        }
        #endregion

        #region Virtual Methods
        #endregion
    }
}
