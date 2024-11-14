using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class EyeSystem : MonoBehaviour
    {
        [SerializeField] private Camera _eyePrefab;
        [SerializeField] private Transform _eyesParent;
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private PlayerMovement _playerMovement;

        private PlayerInputActions _inputActions;
        private InputAction _eyeAction;
        private InputAction _swapCameraAction;
        
        
        private bool _isEyeActive;
        private Camera _eye;
        private bool _cameraIsMain = true;

        private void Awake()
        {
            _inputActions = new PlayerInputActions();
            _eyeAction = _inputActions.Player.Eye;
            _eyeAction.Enable();
            _eyeAction.performed += OnEye;
            _swapCameraAction = _inputActions.Player.SwapCamera;
            _swapCameraAction.Enable();
            _swapCameraAction.performed += OnSwapCamera;
        }

        private void OnSwapCamera(InputAction.CallbackContext obj)
        {
            if (!_isEyeActive)
                return;
            _cameraIsMain = !_cameraIsMain;
            _playerCamera.enabled = _cameraIsMain;
            _eye.enabled = !_cameraIsMain;
            _playerMovement.enabled = _cameraIsMain;
            _eye.GetComponent<MouseLook>().enabled = !_cameraIsMain;
        }

        private void OnDisable()
        {
            _eyeAction.Disable();
            _eyeAction.performed -= OnEye;
        }

        private void OnEye(InputAction.CallbackContext obj)
        {
            _isEyeActive = !_isEyeActive;

            if (_isEyeActive)
            {
                _eye = Instantiate(_eyePrefab, _eyesParent);
                _eye.transform.localPosition = Vector3.zero;
                _eye.transform.localRotation = Quaternion.identity;
                _eye.enabled = false;
                _eye.GetComponent<MouseLook>().enabled = false;
            }
            else
            { 
                Destroy(_eye.gameObject);
            }
        }
    }
}