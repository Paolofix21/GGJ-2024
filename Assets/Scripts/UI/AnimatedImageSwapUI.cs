using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI {
    [RequireComponent(typeof(Image))]
    public class AnimatedImageSwapUI : UIBehaviour {
        #region Public Variables
        [SerializeField] private Sprite[] m_swaps;
        [SerializeField] private float m_interval = 0.4f;
        #endregion

        #region Private Variables
        private Image _image;

        private int _index;
        #endregion

        #region Behaviour Callbacks
        protected override void Awake() => _image = GetComponent<Image>();

        protected override void OnEnable() {
            _index = 0;
            _image.sprite = m_swaps[_index];

            InvokeRepeating(nameof(SwapImages), m_interval, m_interval);
        }

        protected override void OnDisable() => CancelInvoke();
        #endregion

        #region Private Methods
        private void SwapImages() {
            _index = ++_index % m_swaps.Length;
            _image.sprite = m_swaps[_index];
        }
        #endregion
    }
}