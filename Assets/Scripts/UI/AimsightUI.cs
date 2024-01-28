using Miscellaneous;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

namespace UI {
    public class AimsightUI : UIBehaviour {
        public CutsceneIntroController controller = default;

        #region Behaviour Callbacks
        protected override void Awake() {
            controller.OnIntroStartStop += HideShow;
        }

        protected override void OnDestroy() {
            controller.OnIntroStartStop -= HideShow;
            //CutsceneIntroController.singleton.OnIntroStartStop -= HideShow;
        }

        private void HideShow(bool show) => gameObject.SetActive(!show);
        #endregion
    }
}