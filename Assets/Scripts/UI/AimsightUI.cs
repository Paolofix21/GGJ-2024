using Miscellaneous;
using UnityEngine.EventSystems;

namespace UI {
    public class AimsightUI : UIBehaviour {
        #region Behaviour Callbacks
        protected override void Awake() {
            CutsceneIntroController.singleton.OnIntroStartStop += HideShow;
        }

        protected override void OnDestroy() {
            CutsceneIntroController.singleton.OnIntroStartStop -= HideShow;
        }

        private void HideShow(bool show) => gameObject.SetActive(!show);
        #endregion
    }
}