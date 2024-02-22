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
            // WaveSpawner.OnEnemyDeath += DisplayWeaponEnergy;
            // TODO - Bind UI to weapon's energy
            _sword.Shot += DisplayWeaponEnergy;
        }
        private void OnDestroy() {
            if (!_target)
                return;

            _target.OnWeaponChanged -= UpdateWeaponIcon;
            _target.gameObject.GetComponent<PlayerWeaponHandler>().OnUpdateWeaponInfo -= CheckWeapon;
            // WaveSpawner.OnEnemyDeath -= DisplayWeaponEnergy;
            // TODO - Bind UI to weapon's energy
            _sword.Shot -= DisplayWeaponEnergy;
        }
        #endregion

        #region Public Methods
        public void UpdateWeaponIcon(int i)
        {
            m_selectedWeapon.sprite = weaponSprites[i];
        }

        private void CheckWeapon(Weapon weapon) => m_munitions.text = $"{weapon.ChargeStatus.Info}";
        #endregion

        #region Private Methods
        private async void DisplayWeaponEnergy() {
            await Task.Yield();
            // TODO - Bind energy progress to sword weapon
             m_specialWeaponFiller.fillAmount = _sword.CurrentEnergy / 10.0f;
        }
        #endregion

        #region Virtual Methods
        #endregion
    }
}
