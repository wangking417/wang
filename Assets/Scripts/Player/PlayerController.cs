using FirepowerFullBlast.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FirepowerFullBlast.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float sprintMultiplier = 1.35f;
        [SerializeField] private float jumpHeight = 1.25f;
        [SerializeField] private float gravity = -20f;
        [SerializeField] private float lookSensitivity = 1.2f;
        [SerializeField] private float minPitch = -80f;
        [SerializeField] private float maxPitch = 80f;

        private CharacterController _controller;
        private Vector3 _velocity;
        private float _pitch;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            HandleLook();
            HandleMovement();
        }

        private void HandleLook()
        {
            if (Mouse.current == null || playerCamera == null)
            {
                return;
            }

            Vector2 lookDelta = Mouse.current.delta.ReadValue() * lookSensitivity * Time.deltaTime * 60f;

            _pitch = Mathf.Clamp(_pitch - lookDelta.y, minPitch, maxPitch);
            playerCamera.transform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
            transform.Rotate(Vector3.up * lookDelta.x);
        }

        private void HandleMovement()
        {
            if (Keyboard.current == null)
            {
                return;
            }

            Vector2 input = Vector2.zero;
            if (Keyboard.current.wKey.isPressed) input.y += 1f;
            if (Keyboard.current.sKey.isPressed) input.y -= 1f;
            if (Keyboard.current.dKey.isPressed) input.x += 1f;
            if (Keyboard.current.aKey.isPressed) input.x -= 1f;

            input = Vector2.ClampMagnitude(input, 1f);

            float speed = moveSpeed;
            if (Keyboard.current.leftShiftKey.isPressed)
            {
                speed *= sprintMultiplier;
                GameEventBus.RaiseStatusChanged("Sprinting");
            }

            Vector3 move = transform.right * input.x + transform.forward * input.y;
            _controller.Move(move * (speed * Time.deltaTime));

            if (_controller.isGrounded && _velocity.y < 0f)
            {
                _velocity.y = -2f;
            }

            if (_controller.isGrounded && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                GameEventBus.RaiseStatusChanged("Jump");
            }

            _velocity.y += gravity * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
        }
    }
}

