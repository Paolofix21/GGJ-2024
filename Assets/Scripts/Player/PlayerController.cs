using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float speed, jumpForce;
        private const float grav = -9.8f;

        private bool isOnGround;

        private CharacterController controller;
        private PlayerView cameraLook;
        private InputManager input;
        private Vector3 vel;

        private void Start()
        {
            controller = GetComponent<CharacterController>();
            cameraLook = GetComponent<PlayerView>();
            input = GetComponent<InputManager>();

            input.playerMap.PlayerActions.Jump.performed += ctx => Jump();
            input.playerMap.PlayerActions.Crouch.performed += ctx => Crouch();
        }

        private void Update()
        {
            GetMovement();
            cameraLook.GetMousePos(input.CameraLookAt());
        }

        //private void OnCollisionStay(Collision collision)
        //{
        //    if (collision.gameObject.CompareTag("Ground") && !isOnGround)
        //        isOnGround = true;

        //    Debug.Log("colliding");
        //}

        //private void OnCollisionExit(Collision collision)
        //{
        //    if (collision.gameObject.CompareTag("Ground"))
        //        isOnGround = false;

        //    Debug.Log("exit");
        //}

        private void GetMovement()
        {
            Vector3 dir = Vector3.zero;
            dir.x = input.GetMovement().x;
            dir.z = input.GetMovement().y;

            if (controller.isGrounded && vel.y < 0)
            {
                vel.y = -2;
            }
            else
            {
                vel.y += grav * Time.deltaTime;
            }

            controller.Move(transform.TransformDirection(dir) * speed * Time.deltaTime);
            controller.Move(vel * Time.deltaTime);
        }

        private void Jump()
        {
            Debug.Log(controller.isGrounded);
            if (controller.isGrounded)
            {
                vel.y = Mathf.Sqrt(jumpForce * -3 * grav);
                Debug.Log(vel.y);
            }
               
                
        }

        private void Crouch()
        {

        }

    }
}

