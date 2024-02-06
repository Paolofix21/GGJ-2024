using Code.Player;
using Miscellaneous;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI {
    public class AimsightUI : UIBehaviour {
        #region Public Variables
        [SerializeField] private List<GameObject> Aim;
        #endregion

        #region Behaviour Callbacks
        protected override void Awake() {
            CutsceneIntroController.OnIntroStartStop += HideShow;
        }

        protected override void Start() {
            PlayerController.Singleton.OnWeaponChanged += CheckWeapon;
        }

        protected override void OnDestroy() {
            CutsceneIntroController.OnIntroStartStop -= HideShow;
            if(PlayerController.Singleton)
                PlayerController.Singleton.OnWeaponChanged -= CheckWeapon;
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