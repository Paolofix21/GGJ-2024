using UnityEngine;

namespace Code.Graphics {
    [RequireComponent(typeof(MeshRenderer))]
    public class VisualSetter : MonoBehaviour {
        #region Private Variables
        private MeshRenderer _renderer;
        private MaterialPropertyBlock _block;

        private static readonly int MatProp_Hue = Shader.PropertyToID("_Hue");
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            _renderer = GetComponent<MeshRenderer>();

            _block = new MaterialPropertyBlock();
            _block.SetFloat(MatProp_Hue, 0f);

            _renderer.SetPropertyBlock(_block);
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Set the <b>Hue</b> for the target renderer's property block.
        /// </summary>
        /// <param name="value">The hue offset (0 .. 1)</param>
        public void SetHueVal(float value) {
            _renderer.GetPropertyBlock(_block);
            _block.SetFloat(MatProp_Hue, (value - .5f) * 180f);
            _renderer.SetPropertyBlock(_block);
        }

        /// <summary>
        /// Set the <b>Hue</b> for the target renderer's property block.
        /// </summary>
        /// <param name="degrees">The hue offset in degrees (-180° .. 180°)</param>
        public void SetHueDeg(float degrees) {
            _renderer.GetPropertyBlock(_block);
            _block.SetFloat(MatProp_Hue, degrees);
            _renderer.SetPropertyBlock(_block);
        }
        #endregion
    }
}