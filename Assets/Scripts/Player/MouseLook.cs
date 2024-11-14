using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class MouseLook : MonoBehaviour
    {
        [Header("Camera")] [SerializeField] private Transform _playerCamera;
        [SerializeField] private float _sensitivity = 10;

        private PlayerInputActions _playerInputActions;
        private InputAction _lookAction;
        private Vector2 _look;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Awake()
        {
            _playerInputActions = new PlayerInputActions();
        }

        private void OnEnable()
        {
            _lookAction = _playerInputActions.Player.Look;
            _lookAction.Enable();
        }

        private void Update()
        {
            UpdateLook();
        }

        private void UpdateLook()
        {
            _look += _lookAction.ReadValue<Vector2>() * (_sensitivity * Time.deltaTime);
            _look.y = Mathf.Clamp(_look.y, -90, 90);

            transform.rotation = Quaternion.Euler(-_look.y, _look.x, 0);
        }
    }
}