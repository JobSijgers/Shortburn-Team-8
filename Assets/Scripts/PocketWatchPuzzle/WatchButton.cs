using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Player;
using UnityEngine.Events;

namespace PocketWatchPuzzle
{
    public class WatchButton : MonoBehaviour
    {
        [Header("Rotation options")]
        [SerializeField] private GameObject _objectToRotate;
        [SerializeField] private float _timeBetweenRotations;
        [SerializeField] private int _rotationStepInMinutes;
        [SerializeField] private float _rotationSpeed;
        
        [Header("Button options")]
        [SerializeField] private Vector3 _buttonPressedPosition;
        [SerializeField] private AnimationCurve _buttonPressCurve;
        
        [Header("Activation options")]
        [SerializeField] private float _activationDistance;
        [SerializeField] private LayerMask _layerMask;
        
        private PlayerInputActions _playerInputActions;
        private InputAction _useAction;
        private Coroutine _useRoutine;
        
        public UnityAction OnTimeUpdated;
        public Transform GetHand() => _objectToRotate.transform;

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
            if (!CheckDistanceToPlayer() || !CameraController.Instance.IsLookingAtObject(transform, _layerMask)) return;
            if (_useAction.IsPressed() && _useRoutine == null)
            {
                _useRoutine = StartCoroutine(RotateButtonCoroutine());
            }
        }

        private IEnumerator RotateButtonCoroutine()
        {
            while (_useAction.IsPressed())
            {
                StartCoroutine(PressButtonRoutine(transform.position, transform.position + _buttonPressedPosition));
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
            OnTimeUpdated?.Invoke();
            _useRoutine = null;
        }

        private IEnumerator PressButtonRoutine(Vector3 startPosition, Vector3 endPosition, bool doReturn = true)
        {
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * _rotationSpeed * 2;
                float a = _buttonPressCurve.Evaluate(t);
                transform.position = Vector3.Lerp(startPosition, endPosition, a);
                yield return null;
            }
            if (doReturn) StartCoroutine(PressButtonRoutine(endPosition, startPosition, false));
        }
        private Quaternion GetEndRotation()
        {
            float rotationStep = 360f / 60f * _rotationStepInMinutes;
            Quaternion angleToAdd = Quaternion.Euler(0, 0, rotationStep);    
            return _objectToRotate.transform.rotation * angleToAdd;
        }
        private bool CheckDistanceToPlayer()
        {
            return Vector3.Distance(transform.position, PlayerMovement.Instance.transform.position) <= _activationDistance;
        }
    }
}

