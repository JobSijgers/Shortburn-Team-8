using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Camera")] [SerializeField] private Transform _playerCamera;
        [SerializeField] private float _sensitivity = 10;


        [Header("Movement")] [SerializeField] private CharacterController _controller;
        [SerializeField] private float _speed = 5;
        [SerializeField] private float _mass = 1;


        private PlayerInputActions _playerInputActions;
        private InputAction _lookAction;
        private InputAction _moveAction;

        private Vector2 _look;
        private Vector3 _velocity;

        private void Awake()
        {
            _playerInputActions = new PlayerInputActions();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnEnable()
        {
            _lookAction = _playerInputActions.Player.Look;
            _lookAction.Enable();
            _moveAction = _playerInputActions.Player.Move;
            _moveAction.Enable();
        }

        private void Update()
        {
            UpdateLook();
            UpdateMovement();
            UpdateGravity();
        }

        private void UpdateLook()
        {
            _look += _lookAction.ReadValue<Vector2>() * (_sensitivity * Time.deltaTime);
            _look.y = Mathf.Clamp(_look.y, -90, 90);

            transform.localRotation = Quaternion.Euler(0, _look.x, 0);
            _playerCamera.localRotation = Quaternion.Euler(-_look.y, 0, 0);
        }

        private void UpdateMovement()
        {
            Vector2 move = _moveAction.ReadValue<Vector2>();

            Vector3 input = new();
            input += transform.right * move.x;
            input += transform.forward * move.y;
            input = Vector3.ClampMagnitude(input, 1);

            _controller.Move((input * _speed + _velocity) * Time.deltaTime);
        }

        private void UpdateGravity()
        {
            Vector3 gravity = Physics.gravity * (Time.deltaTime * _mass);
            _velocity.y = _controller.isGrounded ? -1f : _velocity.y + gravity.y;
        }
    }
}