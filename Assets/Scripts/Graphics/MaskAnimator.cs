using System.Collections;
using FMODUnity;
using UnityEngine;

namespace Code.Graphics {
    public class MaskAnimator : MonoBehaviour {
        #region Public Variables
        [SerializeField] private ColorSetSO[] m_colorSets;
        [SerializeField] private AnimationCurve m_laughterAnimation;
        [SerializeField] private EventReference m_laughterClipEvent;

        [Space]
        [SerializeField] private int m_laughterShapeIndex;
        #endregion

        #region Private Variables
        private SkinnedMeshRenderer _meshRenderer;
        private TrailRenderer _trailRenderer;

        private MaterialPropertyBlock _block;
        private MaterialPropertyBlock _trailBlock;

        private bool _animatingLaughter;

        private static readonly int MatProp_Hue = Shader.PropertyToID("_Hue");
        private static readonly int MatProp_MainColor = Shader.PropertyToID("_BaseColor");
        private static readonly int MatProp_EmissionColor = Shader.PropertyToID("_EmissionColor");
        #endregion

        #region Properties
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            _meshRenderer = GetComponent<SkinnedMeshRenderer>();
            _trailRenderer = GetComponentInChildren<TrailRenderer>();

            _block = new MaterialPropertyBlock();
            _block.SetFloat(MatProp_Hue, m_colorSets[0].ObjectHue);

            _trailBlock = new MaterialPropertyBlock();
            _trailBlock.SetColor(MatProp_MainColor, m_colorSets[0].TrailColor);
            _trailBlock.SetColor(MatProp_EmissionColor, m_colorSets[0].TrailColor);

            _meshRenderer.SetPropertyBlock(_block);
            _trailRenderer.SetPropertyBlock(_trailBlock);
        }

        [ContextMenu("Set Color/0")]
        private void SetColorZero() => SetColorType(0);
        [ContextMenu("Set Color/1")]
        private void SetColorOne() => SetColorType(1);
        [ContextMenu("Set Color/2")]
        private void SetColorTwo() => SetColorType(2);

        public void SetColorType(int id) {
            var colorSet = m_colorSets[id];
            SetHueDeg(colorSet.ObjectHue);
            SetTrailColor(colorSet.TrailColor);
        }
        #endregion

        #region Public Methods
        [ContextMenu("Laugh")]
        public void AnimateLaughter() {
            if (!isActiveAndEnabled)
                return;

            if (_animatingLaughter)
                return;

            StartCoroutine(LaughCO());
            Debug.Log("Laughing\n");
        }

        #endregion

        #region Private Methods
        private void SetHueDeg(float degrees) {
            _meshRenderer.GetPropertyBlock(_block);
            _block.SetFloat(MatProp_Hue, degrees);
            _meshRenderer.SetPropertyBlock(_block);
        }

        private void SetTrailColor(Color color) {
            _trailRenderer.GetPropertyBlock(_trailBlock);
            _trailBlock.SetColor(MatProp_MainColor, color);
            _trailBlock.SetColor(MatProp_EmissionColor, color);
            _trailRenderer.SetPropertyBlock(_trailBlock);
        }

        private IEnumerator LaughCO() {
            _animatingLaughter = true;

            var t = 0f;
            var duration = m_laughterAnimation.keys[^1].time;

            if (!m_laughterClipEvent.IsNull) {
                var ev = RuntimeManager.CreateInstance(m_laughterClipEvent);
                ev.start();
            }

            while (t < duration) {
                t += Time.deltaTime;
                _meshRenderer.SetBlendShapeWeight(m_laughterShapeIndex, m_laughterAnimation.Evaluate(t) * 100);
                yield return null;
            }

            _meshRenderer.SetBlendShapeWeight(m_laughterShapeIndex, 0f);
            _animatingLaughter = false;
        }
        #endregion

        #region Event Methods
        #endregion
    }
}