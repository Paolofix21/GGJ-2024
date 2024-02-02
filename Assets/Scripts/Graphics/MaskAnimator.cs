using System.Collections;
using FMOD;
using FMODUnity;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Code.Graphics {
    [DefaultExecutionOrder(-1)]
    public class MaskAnimator : MonoBehaviour {
        #region Public Variables
        [SerializeField] private ColorSetSO[] m_colorSets;

        [Header("Intro")]
        [SerializeField] private EventReference m_introVoiceLineEvent;

        [Header("Laughter")]
        [SerializeField] private AnimationCurve m_laughterAnimation;
        [SerializeField] private EventReference m_laughterClipEvent;
        [SerializeField] private int m_laughterShapeIndex;

        [Header("Death")]
        [SerializeField] private AnimationCurve m_deathScaleAnimation;
        [SerializeField] private EventReference m_deathSound;
        [SerializeField] private ParticleSystem m_deathParticle;
        [SerializeField] private float m_deathRotationSpeed = 720f;
        #endregion

        #region Private Variables
        private SkinnedMeshRenderer _meshRenderer;
        private TrailRenderer _trailRenderer;

        private MaterialPropertyBlock _block;
        private MaterialPropertyBlock _trailBlock;

        private Coroutine _deathCoroutine;

        private bool _animatingLaughter;

        private static readonly int MatProp_Hue = Shader.PropertyToID("_Hue");
        private static readonly int MatProp_Saturation = Shader.PropertyToID("_Saturation");
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
            _block.SetFloat(MatProp_Saturation, m_colorSets[0].ObjectSaturation);
            _block.SetColor(MatProp_EmissionColor, m_colorSets[0].EmissionColor);

            _trailBlock = new MaterialPropertyBlock();
            _trailBlock.SetColor(MatProp_MainColor, m_colorSets[0].TrailColor);
            _trailBlock.SetColor(MatProp_EmissionColor, m_colorSets[0].TrailColor);

            _meshRenderer.SetPropertyBlock(_block);
            _trailRenderer.SetPropertyBlock(_trailBlock);
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

        [ContextMenu("Laugh")]
        public void AnimateIntroVoiceLine() {
            var ev = RuntimeManager.CreateInstance(m_introVoiceLineEvent);

            var attr3d = new ATTRIBUTES_3D {
                position = transform.position.ToFMODVector(),
                forward = transform.forward.ToFMODVector()
            };

            ev.set3DAttributes(attr3d);
            ev.start();
        }

        [ContextMenu("Laugh")]
        public void AnimateDeath() {
            if (_deathCoroutine != null)
                return;

            _deathCoroutine = StartCoroutine(DeathCO());

            AudioManager.instance.PlayOneShot(m_deathSound, transform.position);
        }

        public void SetColorType(int id) {
            if (!didAwake)
                Awake();

            var colorSet = m_colorSets[id];
            SetHueDeg(colorSet.ObjectHue, colorSet.ObjectSaturation, colorSet.EmissionColor);
            SetTrailColor(colorSet.TrailColor);
        }
        #endregion

        #region Private Methods
        private void SetHueDeg(float degrees, float saturation, Color color) {
            _block.SetFloat(MatProp_Hue, degrees);
            _block.SetFloat(MatProp_Saturation, saturation);
            _block.SetColor(MatProp_EmissionColor, color);
            _meshRenderer.SetPropertyBlock(_block);
        }

        private void SetTrailColor(Color color) {
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

        private IEnumerator DeathCO() {
            var t = 0f;
            var duration = m_deathScaleAnimation.keys[^1].time;

            while (t < duration) {
                t += Time.deltaTime;
                transform.localScale = Vector3.one * m_deathScaleAnimation.Evaluate(t);
                transform.Rotate(m_deathRotationSpeed * Time.deltaTime * Vector3.up, Space.World);
                yield return null;
            }

            _deathCoroutine = null;
            Instantiate(m_deathParticle, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        #endregion

#if UNITY_EDITOR
        [ContextMenu("Set Color/0")]
        private void SetColorZero() => SetColorType(0);
        [ContextMenu("Set Color/1")]
        private void SetColorOne() => SetColorType(1);
        [ContextMenu("Set Color/2")]
        private void SetColorTwo() => SetColorType(2);
        [ContextMenu("Set Color/3")]
        private void SetColorThree() => SetColorType(3);
#endif
    }
}