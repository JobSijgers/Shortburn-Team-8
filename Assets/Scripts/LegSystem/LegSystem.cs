using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace LegSystem
{
    public class LegSystem : MonoBehaviour
    {
        public static LegSystem Instance;
        [SerializeField] private Leg _legPrefab;
        [SerializeField] private Transform _legOrigin;
        [SerializeField] private float _maxDistanceFromLeg = 10;
        [SerializeField] private PlayerMovement _playerMovement;
        private PlayerInputActions _inputActions;
        private InputAction _legAction;
        public Leg Leg;

        private void Awake()
        {
            Instance = this;
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
            if (Leg == null)
            {
                Leg = Instantiate(_legPrefab, _legOrigin.position, Quaternion.identity);
                _playerMovement.SetConstraint(Leg.transform, _maxDistanceFromLeg);
            }
            else
            {
                Leg.StartLegReturn(_legOrigin, () => Leg = null);
                _playerMovement.RemoveConstraint();
            }
        }
    }
}