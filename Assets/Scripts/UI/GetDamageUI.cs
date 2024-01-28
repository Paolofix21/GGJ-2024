using Code.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI {
    [RequireComponent(typeof(Image))]
    public class GetDamageUI : UIBehaviour {
        #region Private Variables
        private Image _image;
        #endregion

        #region Behaviour Callbacks
        protected override void Awake() {
            _image = GetComponent<Image>();
            _image.CrossFadeAlpha(0f, 0f, true);
        }

        protected override void Start() => PlayerController.Singleton.Health.OnDamageTaken += TakeDamage;
        protected override void OnDestroy() {
            if (PlayerController.Singleton)
                PlayerController.Singleton.Health.OnDamageTaken -= TakeDamage;
        }

        private void TakeDamage(float cur, float max) {
            _image.CrossFadeAlpha(.5f, 0f, true);
            _image.CrossFadeAlpha(0f, .5f, true);
        }
        #endregion
    }
}