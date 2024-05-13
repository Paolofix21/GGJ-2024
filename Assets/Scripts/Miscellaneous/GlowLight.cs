using UnityEngine;

namespace Miscellaneous {
    [RequireComponent(typeof(MeshRenderer))]
    public class GlowLight : MonoBehaviour {
        #region Public Variables
        [SerializeField] private AnimationCurve m_glowPattern = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        #endregion

        #region Private Variables
        private MeshRenderer _renderer;
        private Color _color;

        private static readonly int MatProp_Color = Shader.PropertyToID("_Color");
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            _renderer = GetComponent<MeshRenderer>();
            _color = _renderer.material.GetColor(MatProp_Color);
        }

        private void Update() {
            var block = new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(block);
            var targetColor = Color.Lerp(Color.black, _color, m_glowPattern.Evaluate(Time.time));
            block.SetColor(MatProp_Color, targetColor);
            _renderer.SetPropertyBlock(block);
        }
        #endregion
    }
}