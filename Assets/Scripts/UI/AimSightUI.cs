using Code.Player;
using Miscellaneous;
using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.UI {
    public sealed class AimSightUI : UIBehaviour {
        #region Public Variables
        [SerializeField] private List<GameObject> Aim;
        #endregion

        private PlayerController _player;

        #region Behaviour Callbacks
        protected override void Awake() {
            CutsceneIntroController.OnIntroStartStop += HideShow;
        }

        protected override void Start() {
            _player = GameEvents.MatchManager.GetPlayerEntity().Transform.GetComponent<PlayerController>();
            _player.OnWeaponChanged += CheckWeapon;
        }

        protected override void OnDestroy() {
            CutsceneIntroController.OnIntroStartStop -= HideShow;
            if(_player)
                _player.OnWeaponChanged -= CheckWeapon;
        }

        private void HideShow(bool show) => gameObject.SetActive(!show);
        #endregion

        #region Private Methods
        private void CheckWeapon(int weapon) {
            foreach (var item in Aim)
                item.SetActive(false);
            Aim[weapon].SetActive(true);
        }
        #endregion
    }
}