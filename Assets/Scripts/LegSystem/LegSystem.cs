using Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LegSystem
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
            _legAction.performed += LeaveLeg;
        }

        private void OnDisable()
        {
            _legAction.Disable();
            _legAction.performed -= LeaveLeg;
        }

        private void LeaveLeg(InputAction.CallbackContext obj)
        {
            if (_leg == null)
            {
                _leg = Instantiate(_legPrefab, _legOrigin.position, Quaternion.identity);
                _playerMovement.SetConstraint(_leg.transform, _maxDistanceFromLeg);
            }
            else
            {
                _leg.StartLegReturn(_legOrigin);
                _leg = null;
                _playerMovement.RemoveConstraint();
            }
        }
    }
}