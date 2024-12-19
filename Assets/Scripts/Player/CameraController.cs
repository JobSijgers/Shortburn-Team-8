using System;
using System.Collections;
using System.Collections.Generic;
using LegSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using LimbsPickup;
namespace Player
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance;
        [Header("Camera rotations")]
        [SerializeField] private float _camMaxRotateAngleZ;
        [SerializeField] private float _camMaxRotateAngleX;
        
        [Header("Movement settings")]
        [SerializeField] private float _sensitivity;

        [Header("View bobbing")] 
        [SerializeField] private bool _doViewBobbing;
        [SerializeField] private float _bobFrequency = 14f;
        [SerializeField] private float _bobIntensityX = 0.05f;
        [SerializeField] private float _bobIntensityY = 0.05f;
        [SerializeField] private float _leglessBobIntensityY = 0.05f;
        [SerializeField] private float _runMultiplier;

        [Header("FOV")] 
        [SerializeField] private float _walkFOV;
        [SerializeField] private float _runFOV;
        [SerializeField] private float _fovChangeDuration; 

        private Camera _camera;
        private Dictionary<EPlayerMovementState, float> FOVS = new();
        private Coroutine fovCoroutine;
        
        private PlayerMovement _player;
        private Vector2 _look;
        private float _xRotation;
        private Vector3 _defaultPos; 
        private float _timer = 0;

        private void Awake()
        {   
            Instance = this;
            _camera = GetComponent<Camera>();
        }

        private void Start()
        {
            _player = PlayerMovement.Instance;
            _player.OnPlayerStateChange += () => ChangeFOV(_player.PlayerMovementState);
            FOVS.Add(EPlayerMovementState.Walking, _walkFOV);
            FOVS.Add(EPlayerMovementState.Running, _runFOV);
            _defaultPos = transform.localPosition;
        }

        private void Update()
        {
            if (!_doViewBobbing)
                return;
            float multiplier = _player.PlayerMovementState == EPlayerMovementState.Running ? _runMultiplier : 1;
            if ((Mathf.Abs(_player.MoveVector.x) > 0.1f || Mathf.Abs(_player.MoveVector.z) > 0.1f) && _player.Grounded())
            {
                _timer += Time.deltaTime * (_bobFrequency * multiplier);
                float intensityY = LimbsController.Instance.LegState ^ Leg.Instance ? _bobIntensityY : _leglessBobIntensityY;
                float bobX = Mathf.Cos(_timer * _bobFrequency / 23) * (_bobIntensityX / 60);
                float bobY = Mathf.Sin(_timer) * intensityY;

                transform.localPosition = new Vector3(transform.localPosition.x + bobX, _defaultPos.y + bobY, transform.localPosition.z);
            }
            else
            {
                _timer = 0;
                transform.localPosition = new Vector3(Mathf.Lerp(transform.localPosition.x, _defaultPos.x, Time.deltaTime * _bobFrequency), Mathf.Lerp(transform.localPosition.y, _defaultPos.y, Time.deltaTime * _bobFrequency), _defaultPos.z);
            }
        }

        public void UpdateLook(InputAction action, Transform playerBody)
        {
            _look = action.ReadValue<Vector2>() * (_sensitivity * Time.deltaTime);
            _xRotation -= _look.y;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
            
            Quaternion endRot = Quaternion.Euler(_player.Y * _camMaxRotateAngleZ, 0, _player.X * -_camMaxRotateAngleX);
            Quaternion xRotationQuaternion = Quaternion.Euler(_xRotation, 0f, 0f);
            endRot = xRotationQuaternion * endRot;

            transform.localRotation = Quaternion.Slerp(transform.localRotation, endRot, 1);
            playerBody.Rotate(Vector3.up * _look.x);
        }
        private void ChangeFOV(EPlayerMovementState newState)
        {
            if (FOVS.ContainsKey(newState))
            {
                if (fovCoroutine != null) StopCoroutine(fovCoroutine);
                fovCoroutine = StartCoroutine(ChangeFOVCoroutine(FOVS[newState], _fovChangeDuration));
            }
        }
        public IEnumerator ChangeFOVCoroutine(float newFov, float duration)
        {
            float t = 0;
            float start = _camera.fieldOfView;
            while (t < duration)
            {
                t += Time.deltaTime;
                float prc = t / duration;
                _camera.fieldOfView = Mathf.Lerp(start, newFov, prc);
                yield return null;
            }
            fovCoroutine = null;
        }
        public bool IsLookingAtObject(Transform obj, LayerMask mask)
        {
            return Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, Mathf.Infinity, mask) && hit.transform == obj;
        }
    }
}

