using Cinemachine;
using Code.Data;
using Code.Graphics;
using Code.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Code.Player
{
    public class PlayerView : MonoBehaviour
    {
        [Header("Sensitivity")]
        [SerializeField] private float sensitivityX = 30;
        [SerializeField] private float sensitivityY = 30;

        [Header("Rotation Angle")]
        [SerializeField] private float maxRot;
        [SerializeField] private float minRot;

        [Header("Bobbing")]
        [SerializeField] private float speed;
        [SerializeField] private float amount;
        private float timer;

        [SerializeField] private GameObject cam;
        private CinemachineVirtualCamera vcam;
        private CinemachineTransposer transposer;
        private CharacterController controller;
        private Volume globalVolume;

        private float xrot = 0f;
        private float yrot = 0f;

        private float defaultY = .7f;

        private void Awake()
        {
            vcam = cam.GetComponent<CinemachineVirtualCamera>();
            transposer = vcam.GetCinemachineComponent<CinemachineTransposer>();
            controller = GetComponent<CharacterController>();
            globalVolume = FindFirstObjectByType<Volume>();
        }
        private void Start() {
            VideoSettingsHelper.FOV = DataManager.GetGamePlaySetting<int>(GamePlaySettings.Type.FieldOfView);
            VideoSettingsHelper.MouseSensitivity = DataManager.GetGamePlaySetting<int>(GamePlaySettings.Type.Sensitivity);
            VideoSettingsHelper.MotionBlurActive = DataManager.GetVideoSetting<bool>(VideoSettings.Type.MotionBlur);

            OnSensitivity(VideoSettingsHelper.MouseSensitivity);
            OnFOV(VideoSettingsHelper.FOV);
            OnMotionBlur(VideoSettingsHelper.MotionBlurActive);

            SettingsUI.OnSensitivityChanged += OnSensitivity;
            SettingsUI.OnFOVChanged += OnFOV;
            SettingsUI.OnMotionBlurChanged += OnMotionBlur;
        }
        private void Update()
        {
            DoHeadBobbing();
        }
        private void OnDestroy()
        {
            SettingsUI.OnSensitivityChanged -= OnSensitivity;
            SettingsUI.OnFOVChanged -= OnFOV;
            SettingsUI.OnMotionBlurChanged -= OnMotionBlur;
        }
        public void GetMousePos(Vector2 input)
        {
            float mousex = input.x;
            float mousey = input.y;

            xrot -= (mousey * Time.deltaTime) * sensitivityY;
            xrot = Mathf.Clamp(xrot, minRot, maxRot);
            cam.transform.localRotation = Quaternion.Euler(xrot, 0,0);
            cam.transform.Rotate(Vector3.up * (mousex * Time.deltaTime) * sensitivityX);

            yrot += (mousex * Time.deltaTime) * sensitivityX;
            transform.rotation = Quaternion.Euler(0, yrot, 0);
        }

        #region Crouching [NOT USED]
        //public async void ChangeViewHeight(bool _value)
        //{
        //    var transposer = vcam.GetCinemachineComponent<CinemachineTransposer>();

        //    var standingView = new Vector3(0, 0.7f, 0.9f); 
        //    var crouchedView = new Vector3(0, -0.3f, 0.9f);

        //    var _time = 0f;

        //    Vector3 valueToReach;
        //    Vector3 transposerPos = transposer.m_FollowOffset;

        //    if (_value)
        //        valueToReach = crouchedView;
        //    else
        //        valueToReach = standingView;

        //    while(_time < crouchingTime)
        //    {
        //        transposer.m_FollowOffset = Vector3.Lerp(transposerPos, valueToReach, _time / crouchingTime);
        //        _time += Time.deltaTime;
        //        await Task.Yield();
        //    }
        //}
        #endregion

        private void DoHeadBobbing()
        {
            if (controller.velocity.sqrMagnitude > 0.0001f)
            {
                timer += Time.deltaTime * speed;
                transposer.m_FollowOffset = new Vector3(transposer.m_FollowOffset.x, defaultY + Mathf.Sin(timer) * amount, transposer.m_FollowOffset.z);
            }
            else
            {
                timer = 0;
                transposer.m_FollowOffset = new Vector3(transposer.m_FollowOffset.x, Mathf.Lerp(transposer.m_FollowOffset.y, defaultY, Time.deltaTime * speed), transposer.m_FollowOffset.z);
            }

        }

        private void OnSensitivity(int value)
        {
            sensitivityX = value;
            sensitivityY = value;
            Debug.Log($"sensitivity {value}");
        }
        private void OnFOV(int value)
        {
            vcam.m_Lens.FieldOfView = value;
            Debug.Log($"fov {value}");
        }
        private void OnMotionBlur(bool value)
        {
            MotionBlur blur;
            if (globalVolume.profile.TryGet(out blur))
            {
                blur.active = value;
                Debug.Log($"blur {value}");
            }
        }
    }
}

