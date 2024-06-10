using Cinemachine;
using Code.Data;
using Code.Graphics;
using Code.UI;
using Miscellaneous;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Code.Player {
    public class PlayerView : MonoBehaviour {
        #region Public Variables
        [Header("Sensitivity")]
        [SerializeField] private float sensitivityX = 30;
        [SerializeField] private float sensitivityY = 30;

        [Header("Rotation Angle")]
        [SerializeField] private float maxRot = 70f;
        [SerializeField] private float minRot = -70f;

        [Header("Bobbing")]
        [SerializeField] private float speed;
        [SerializeField] private float amount;

        [Space]
        [SerializeField] private GameObject cam;
        #endregion

        #region Private Variables
        private Camera _camera;
        private Rigidbody _body;
        private Volume _globalVolume;

        private Vector3 _transposer;
        private float _timer;
        private float _xrot;
        private float _yrot;

        private float _defaultY = .7f;
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            _camera = GetComponent<Camera>();
            _body = GetComponentInParent<Rigidbody>();
            _globalVolume = FindFirstObjectByType<Volume>();
        }

        private void Start() {
            VideoSettingsHelper.FOV = DataManager.GetGamePlaySetting<int>(GamePlaySettings.Type.FieldOfView);
            VideoSettingsHelper.MouseSensitivity = DataManager.GetGamePlaySetting<int>(GamePlaySettings.Type.Sensitivity);
            VideoSettingsHelper.MotionBlurActive = DataManager.GetVideoSetting<bool>(VideoSettings.Type.MotionBlur);
            VideoSettingsHelper.VideoQuality = DataManager.GetVideoSetting<int>(VideoSettings.Type.VideoQuality);

            OnSensitivitySetting(VideoSettingsHelper.MouseSensitivity);
            OnFOVSetting(VideoSettingsHelper.FOV);
            OnMotionBlurSetting(VideoSettingsHelper.MotionBlurActive);
            OnGraphicQualitySetting(VideoSettingsHelper.VideoQuality);

            SettingsUI.OnSensitivityChanged += OnSensitivitySetting;
            SettingsUI.OnFOVChanged += OnFOVSetting;
            SettingsUI.OnMotionBlurChanged += OnMotionBlurSetting;
            SettingsUI.OnGraphicsQualityChanged += OnGraphicQualitySetting;
        }

        private void Update() => DoHeadBobbing();

        private void FixedUpdate() => transform.localRotation = Quaternion.Euler(_xrot, _yrot, 0f);

        private void OnDestroy() {
            SettingsUI.OnSensitivityChanged -= OnSensitivitySetting;
            SettingsUI.OnFOVChanged -= OnFOVSetting;
            SettingsUI.OnMotionBlurChanged -= OnMotionBlurSetting;
            SettingsUI.OnGraphicsQualityChanged -= OnGraphicQualitySetting;
        }
        #endregion

        public void ApplyMotion(Vector2 input) {
            var mouseX = input.x;
            var mouseY = input.y;

            _xrot = Mathf.Clamp(_xrot - mouseY * sensitivityY * .01f, minRot, maxRot);
            _yrot = Mathf.Repeat(_yrot + mouseX * sensitivityX * .01f, 360f);
        }

        #region Private Methods
        private void DoHeadBobbing() {
            var flatVel = _body.velocity;
            flatVel.y = 0f;

            if (flatVel.sqrMagnitude > 0.1f) {
                _timer += Time.deltaTime * speed;
                _transposer = new Vector3(_transposer.x, _defaultY + Mathf.Sin(_timer) * amount, _transposer.z);
            }
            else {
                _timer = 0;
                _transposer = new Vector3(_transposer.x, Mathf.Lerp(_transposer.y, _defaultY, Time.deltaTime * speed), _transposer.z);
            }

            transform.localPosition = _transposer;
        }
        #endregion

        #region Event Methods
        private void OnSensitivitySetting(int value) {
            sensitivityX = value;
            sensitivityY = value;
        }

        private void OnFOVSetting(int value) => _camera.fieldOfView = value;

        private void OnMotionBlurSetting(bool value) {
            if (!_globalVolume.profile.TryGet(out MotionBlur blur))
                return;

            blur.active = value;
        }

        private void OnGraphicQualitySetting(int qualityIndex) {
            OnMotionBlurSetting(VideoSettingsHelper.MotionBlurActive);
            _globalVolume.profile = GraphicsManager.Singleton.GetVolumeProfile(qualityIndex);
        }
        #endregion
    }
}