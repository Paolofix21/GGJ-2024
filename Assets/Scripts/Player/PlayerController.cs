using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Player
{
    public class PlayerController : MonoBehaviour
    {
        #region Movement Fields
        [SerializeField] private float speed, jumpForce;

        private const float grav = -9.8f;
        private bool crouching = false;

        private Vector3 vel;

        [SerializeField] private Animator anim;
        private CharacterController controller;
        private PlayerView cameraLook;
        private InputManager input;
        #endregion

        #region Animations
        private static readonly int isShooting = Animator.StringToHash("Is Shooting");
        private static readonly int shootTrigger = Animator.StringToHash("Shoot");
        private static readonly int weaponType = Animator.StringToHash("Weapon Type Index");
        #endregion

        private void Start()
        {
            controller = GetComponent<CharacterController>();
            cameraLook = GetComponent<PlayerView>();
            input = GetComponent<InputManager>();

            input.playerMap.PlayerActions.Jump.performed +=  Jump;
            input.playerMap.PlayerActions.Crouch.performed += Crouch;
            input.playerMap.PlayerActions.Shoot.performed += PlayShoot;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnDestroy()
        {
            input.playerMap.PlayerActions.Jump.performed -= Jump;
            input.playerMap.PlayerActions.Crouch.performed -= Crouch;
            input.playerMap.PlayerActions.Shoot.performed -= PlayShoot;
        }

        private void Update()
        {
            GetMovement();
            cameraLook.GetMousePos(input.CameraLookAt());
        }

        #region Movement Behaviours
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

        private void Jump(InputAction.CallbackContext ctx)
        {
            Debug.Log(controller.isGrounded);
            if (controller.isGrounded)
            {
                vel.y = Mathf.Sqrt(jumpForce * -3 * grav);

                if(crouching)
                    Crouch(ctx);
            }
        }

        private void Crouch(InputAction.CallbackContext ctx)
        {
            crouching = !crouching;
            cameraLook.ChangeViewHeight(crouching);
        }
        #endregion

        #region Animation Behaviours
        private void SetWeaponType(int type)
        {
            anim.SetInteger(weaponType, type);
        }

        private void PlayShoot(InputAction.CallbackContext ctx)
        {
            anim.SetTrigger(shootTrigger);
        }

        private void PlayShootContinuous(bool _value)
        {
            if (_value)
                anim.SetBool(isShooting, true);
            else
                anim.SetBool(isShooting, false);
        }
        #endregion
    }
}

