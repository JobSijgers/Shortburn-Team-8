using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Player;

namespace PocketWatch
{
    public class WatchButton : MonoBehaviour
    {
        [Header("Rotation options")]
        [SerializeField] private GameObject _objectToRotate;
        [SerializeField] private float _timeBetweenRotations;
        [SerializeField] private int _rotationStepInMinutes;
        [SerializeField] private float _rotationSpeed;
        
        [Header("Activation options")]
        [SerializeField] private float _activationDistance;
        
        private PlayerInputActions _playerInputActions;
        private InputAction _useAction;
        private Coroutine _useRoutine;

        private void Awake()
        {
            _playerInputActions = new PlayerInputActions();
            _useAction = _playerInputActions.Player.Use;
        }
        
        private void OnEnable()
        {
            _useAction.Enable();
        }

        private void Update()
        {
            if (DistanceToPlayer() > _activationDistance) return;
            if (_useAction.IsPressed() && _useRoutine == null)
            {
                _useRoutine = StartCoroutine(RotateButtonCoroutine());
            }
        }

        private IEnumerator RotateButtonCoroutine()
        {
            while (_useAction.IsPressed())
            {
                Quaternion startRotation = _objectToRotate.transform.rotation;
                Quaternion endRotation = GetEndRotation();
                float t = 0;
                while (t < 1)
                {
                    t += Time.deltaTime * _rotationSpeed;
                    _objectToRotate.transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
                    yield return null;
                }
                yield return new WaitForSeconds(_timeBetweenRotations);
                yield return null;
            }
            _useRoutine = null;
        }
        private Quaternion GetEndRotation()
        {
            float rotationStep = 360f / 60f * _rotationStepInMinutes;
            Quaternion angleToAdd = Quaternion.Euler(0, 0, rotationStep);    
            return _objectToRotate.transform.rotation * angleToAdd;
        }
        private float DistanceToPlayer()
        {
            return Vector3.Distance(transform.position, PlayerMovement.Instance.transform.position);
        }
    }
}

