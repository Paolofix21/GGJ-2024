using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI {
    [RequireComponent(typeof(Image))]
    public class AnimatedProgressUI : UIBehaviour {
        #region Public Variables
        [SerializeField] private float m_quickness = 2f;
        #endregion

        #region Private Variables
        private Image _image;
        #endregion

        #region Behaviour Callbacks
        protected override void Awake() => _image = GetComponent<Image>();

        private void Update() => _image.fillAmount = Mathf.Repeat(Time.time, 1f);
        #endregion
    }
}