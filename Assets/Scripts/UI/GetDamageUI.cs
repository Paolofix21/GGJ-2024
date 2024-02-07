using Code.Core;
using Code.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI {
    [RequireComponent(typeof(Image))]
    public class GetDamageUI : UIBehaviour {
        #region Private Variables
        private Image _image;
        private PlayerController _target;
        #endregion

        #region Behaviour Callbacks
        protected override void Awake() {
            _image = GetComponent<Image>();
            _image.CrossFadeAlpha(0f, 0f, true);
        }

        protected override void Start() {
            _target = GameEvents.MatchManager.GetPlayerEntity().Transform.GetComponent<PlayerController>();
            _target.Health.OnDamageTaken += TakeDamage;
        }

        protected override void OnDestroy() {
            if (_target)
                _target.Health.OnDamageTaken -= TakeDamage;
        }

        private void TakeDamage(float cur, float max) {
            _image.CrossFadeAlpha(.5f, 0f, true);
            _image.CrossFadeAlpha(0f, .5f, true);
        }
        #endregion
    }
}