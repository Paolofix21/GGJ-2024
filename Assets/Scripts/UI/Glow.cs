using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI {
    public class Glow : UIBehaviour {
        #region Public Variables
        [SerializeField] private Graphic m_graphic;
        [SerializeField] private AnimationCurve m_animation;
        #endregion

        #region Private Variables
        private Color _color;
        private Color _alpha;
        #endregion

        #region Behaviour Callbacks
        protected override void Awake() {
            _color = m_graphic.color;
            _alpha = m_graphic.color;
            _alpha.a = 0f;
        }

        private void Update() => m_graphic.color = Color.Lerp(_color, _alpha, m_animation.Evaluate(Time.time));
        #endregion
    }
}