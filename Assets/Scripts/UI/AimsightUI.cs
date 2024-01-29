using Miscellaneous;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

namespace UI {
    public class AimsightUI : UIBehaviour {

        #region Behaviour Callbacks
        protected override void Awake() {
            CutsceneIntroController.OnIntroStartStop += HideShow;
        }

        protected override void OnDestroy() {
            CutsceneIntroController.OnIntroStartStop -= HideShow;
            //CutsceneIntroController.singleton.OnIntroStartStop -= HideShow;
        }

        private void HideShow(bool show) => gameObject.SetActive(!show);
        #endregion
    }
}