using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class LegSystem : MonoBehaviour
    {
        [SerializeField] private Leg _legPrefab;
        [SerializeField] private Transform _legOrigin;
        [SerializeField] private float _maxDistanceFromLeg = 10;
        [SerializeField] private PlayerMovement _playerMovement;
        private PlayerInputActions _inputActions;
        private InputAction _legAction;
        private Leg _leg;

        private void Awake()
        {
            _inputActions = new PlayerInputActions();
        }

        private void OnEnable()
        {
            _legAction = _inputActions.Player.Leg;
            _legAction.Enable();
            _legAction.performed += OnLeg;
        }

        private void OnDisable()
        {
            _legAction.Disable();
            _legAction.performed -= OnLeg;
        }

        private void OnLeg(InputAction.CallbackContext obj)
        {
            if (_leg == null)
            {
                _leg = Instantiate(_legPrefab, transform.position, Quaternion.identity);
                _playerMovement.SetConstraint(_leg.transform, _maxDistanceFromLeg);
            }
            else
            {
                _leg.ReturnLeg(_legOrigin.position);
                _leg = null;
                _playerMovement.RemoveConstraint();
            }
        }
    }
}