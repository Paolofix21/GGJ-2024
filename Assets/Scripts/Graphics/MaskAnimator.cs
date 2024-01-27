using System.Collections;
using FMODUnity;
using UnityEngine;

namespace Code.Graphics {
    public class MaskAnimator : MonoBehaviour {
        #region Public Variables
        [SerializeField] private AnimationCurve m_laughterAnimation;
        [SerializeField] private EventReference m_laughterClipEvent;

        [Space]
        [SerializeField] private int m_laughterShapeIndex;
        #endregion

        #region Private Variables
        private SkinnedMeshRenderer _meshRenderer;
        private MaterialPropertyBlock _block;

        private bool _animatingLaughter;

        private static readonly int MatProp_Hue = Shader.PropertyToID("_Hue");
        #endregion

        #region Properties
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            _meshRenderer = GetComponent<SkinnedMeshRenderer>();

            _block = new MaterialPropertyBlock();
            _block.SetFloat(MatProp_Hue, 0f);

            _meshRenderer.SetPropertyBlock(_block);
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

        /// <summary>
        ///     Set the <b>Hue</b> for the target renderer's property block.
        /// </summary>
        /// <param name="value">The hue offset (0 .. 1)</param>
        public void SetHueVal(float value) {
            _meshRenderer.GetPropertyBlock(_block);
            _block.SetFloat(MatProp_Hue, (value - .5f) * 180f);
            _meshRenderer.SetPropertyBlock(_block);
        }

        /// <summary>
        /// Set the <b>Hue</b> for the target renderer's property block.
        /// </summary>
        /// <param name="degrees">The hue offset in degrees (-180° .. 180°)</param>
        public void SetHueDeg(float degrees) {
            _meshRenderer.GetPropertyBlock(_block);
            _block.SetFloat(MatProp_Hue, degrees);
            _meshRenderer.SetPropertyBlock(_block);
        }
        #endregion

        #region Private Methods
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