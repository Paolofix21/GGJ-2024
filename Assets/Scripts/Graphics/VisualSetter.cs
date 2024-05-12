using System.Collections;
using UnityEngine;

namespace Code.Graphics {
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    public class VisualSetter : MonoBehaviour {
        #region Public Variables
        [SerializeField] private AnimationCurve m_powerRemapping = AnimationCurve.EaseInOut(0f, 1f, 1f, 1f);
        [SerializeField] private AnimationCurve m_glowProgress = AnimationCurve.EaseInOut(0f, 0f, 1f, 0f);
        #endregion

        #region Private Variables
        private SkinnedMeshRenderer _renderer;
        private MaterialPropertyBlock _block;

        private Coroutine _glowCoroutine;

        private static readonly int MatProp_Hue = Shader.PropertyToID("_Hue");
        private static readonly int MatProp_EmissivePower = Shader.PropertyToID("_Emissive_Power");
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            _renderer = GetComponent<SkinnedMeshRenderer>();

            _block = new MaterialPropertyBlock();
            _block.SetFloat(MatProp_Hue, 0f);
            _block.SetFloat(MatProp_EmissivePower, 1f);

            _renderer.SetPropertyBlock(_block);
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Set the <b>Hue</b> for the target renderer's property block.
        /// </summary>
        /// <param name="value">The hue offset (0 .. 1)</param>
        public void SetHueVal(float value) {
            if (!_renderer)
                return;

            _renderer.GetPropertyBlock(_block);
            _block.SetFloat(MatProp_Hue, (value - .5f) * 180f);
            _renderer.SetPropertyBlock(_block);
        }

        /// <summary>
        /// Set the <b>Hue</b> for the target renderer's property block.
        /// </summary>
        /// <param name="degrees">The hue offset in degrees (-180° .. 180°)</param>
        public void SetHueDeg(float degrees) {
            if (!_renderer)
                return;

            _renderer.GetPropertyBlock(_block);
            _block.SetFloat(MatProp_Hue, degrees);
            _renderer.SetPropertyBlock(_block);
        }

        public void SetEmissivePower(float power) {
            if (!_renderer)
                return;

            _renderer.GetPropertyBlock(_block);
            _block.SetFloat(MatProp_EmissivePower, m_powerRemapping.Evaluate(power));
            _renderer.SetPropertyBlock(_block);
        }

        public void AnimateGlow(float withPower) {
            if (_glowCoroutine != null)
                StopCoroutine(_glowCoroutine);
            _glowCoroutine = StartCoroutine(GlowCO(withPower));
        }
        #endregion

        #region Private Variables
        private IEnumerator GlowCO(float withPower) {
            _renderer.GetPropertyBlock(_block);

            var t = 0f;
            var duration = m_glowProgress.keys[^1].time;
            while (t < duration) {
                t += Time.deltaTime;
                _block.SetFloat(MatProp_EmissivePower, m_glowProgress.Evaluate(t / duration) * withPower);
                _renderer.SetPropertyBlock(_block);
                yield return null;
            }

            _block.SetFloat(MatProp_EmissivePower, 0f);
            _renderer.SetPropertyBlock(_block);
        }
        #endregion
    }
}