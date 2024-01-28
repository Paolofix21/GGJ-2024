using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Advepa.Runtime.Agora.UI.Graphics {
    [RequireComponent(typeof(CanvasRenderer))]
    [ExecuteInEditMode]
    public class GraphicToggle : Graphic {
        #region Internal Classes
        [System.Serializable]
        private class GraphicInfo {
            [field: SerializeField] private Graphic Graphic { get; set; } = null;
            [field: SerializeField] private float ShownAlpha { get; set; } = 0f;
            [field: SerializeField] private float HiddenAlpha { get; set; } = 1f;
            [field: SerializeField] private Color ShownColor { get; set; } = Color.white;
            [field: SerializeField] private Color HiddenColor { get; set; } = Color.white;

            private MonoBehaviour _mono;
            private Color _fromColor, _toColor;
            private Coroutine _fadingCoroutine;

            public void Init(MonoBehaviour mono) => _mono = mono;

            public void Set(bool show) {
                if (!Graphic)
                    return;

                Graphic.CrossFadeAlpha(show ? ShownAlpha : HiddenAlpha, 0f, true);
                Graphic.color = show ? HiddenColor : ShownColor;
            }

            public void DoCrossFade(bool show, float duration, bool ignoreTimeScale) {
                Graphic?.CrossFadeAlpha(show ? ShownAlpha : HiddenAlpha, duration, ignoreTimeScale);
                CrossFadeColor(show, duration, ignoreTimeScale);
                // Graphic?.CrossFadeColor(shownHidden ? ShownColor : HiddenColor, duration, ignoreTimeScale, false);
            }

            public void CrossFadeAlpha(bool show, float duration, bool ignoreTimeScale) => Graphic?.CrossFadeAlpha(show ? ShownAlpha : HiddenAlpha, duration, ignoreTimeScale);

            public void CrossFadeColor(bool show, float duration, bool ignoreTimeScale) {
                _fromColor = show ? ShownColor : HiddenColor;
                _toColor = show ? HiddenColor : ShownColor;

                if (_fadingCoroutine != null)
                    _mono.StopCoroutine(_fadingCoroutine);

                if (duration <= 0f) {
                    Graphic.color = _toColor;
                    return;
                }

                _fadingCoroutine = _mono.StartCoroutine(FadeCO(duration, ignoreTimeScale));
            }

            private IEnumerator FadeCO(float duration, bool ignoreTimeScale) {
                var t = 0f;
                while (t < duration) {
                    t += ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;

                    Graphic.color = Color.Lerp(_fromColor, _toColor, t / duration);
                    yield return null;
                }

                Graphic.color = _toColor;
            }
        }
        #endregion

        #region Public Variables
        [SerializeField] private List<GraphicInfo> m_childGraphics = new List<GraphicInfo>();
        #endregion

        #region Private Variables
        #endregion

        #region Properties
        #endregion

        #region Overrides
        protected override void Awake() => m_childGraphics.ForEach(g => g.Init(this));

#if UNITY_EDITOR
        private void Update() {
            if (UnityEditor.EditorApplication.isPlaying)
                return;

            var ren = GetComponent<CanvasRenderer>();
            var alpha = ren.GetAlpha();
            m_childGraphics.ForEach(g => g.Set(alpha >= .1f));
        }
#endif

        public override void CrossFadeAlpha(float alpha, float duration, bool ignoreTimeScale) {
            m_childGraphics.ForEach(g => g.DoCrossFade(alpha >= .1f, duration, ignoreTimeScale));
        }
        #endregion

        #region Behaviour Callbacks
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        #endregion
    }
}