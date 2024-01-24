using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Player
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private float x = 30;
        [SerializeField] private float y = 30;

        [SerializeField] private GameObject cam;

        private float xrot = 0f;
        private float yrot = 0f;

        public void GetMousePos(Vector2 input)
        {
            float mousex = input.x;
            float mousey = input.y;

            xrot -= (mousey * Time.deltaTime) * y;
            xrot = Mathf.Clamp(xrot, -80, 80);
            cam.transform.localRotation = Quaternion.Euler(xrot, 0,0);
            cam.transform.Rotate(Vector3.up * (mousex * Time.deltaTime) * x);

            yrot += (mousex * Time.deltaTime) * x;
            //transform.Rotate(0, yrot, 0);
            transform.rotation = Quaternion.Euler(0, yrot, 0);
        }
    }
}

