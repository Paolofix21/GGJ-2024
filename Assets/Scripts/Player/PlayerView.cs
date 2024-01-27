using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

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

        [SerializeField, Space(10)] private float crouchingTime = .3f;

        [SerializeField] private GameObject cam;
        private CinemachineVirtualCamera vcam;

        private float xrot = 0f;
        private float yrot = 0f;

        private void Start()
        {
            vcam = cam.GetComponent<CinemachineVirtualCamera>();
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

        public async void ChangeViewHeight(bool _value)
        {
            var transposer = vcam.GetCinemachineComponent<CinemachineTransposer>();

            var standingView = new Vector3(0, 0.7f, 0.9f); 
            var crouchedView = new Vector3(0, -0.3f, 0.9f);

            var _time = 0f;

            Vector3 valueToReach;
            Vector3 transposerPos = transposer.m_FollowOffset;

            if (_value)
                valueToReach = crouchedView;
            else
                valueToReach = standingView;

            while(_time < crouchingTime)
            {
                transposer.m_FollowOffset = Vector3.Lerp(transposerPos, valueToReach, _time / crouchingTime);
                _time += Time.deltaTime;
                await Task.Yield();
            }
            


        }
    }
}

