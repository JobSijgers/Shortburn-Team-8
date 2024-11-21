using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ArmSystem
{
    public class ArmShoot : MonoBehaviour
    {
        [SerializeField] private Arm _armPrefab;
        [SerializeField] private Transform _armSpawnPosition;
        [SerializeField] private Transform _armHoldPosition;
        private PlayerInputActions _playerInputActions;
        private InputAction _shootAction;
        private InputAction _pullAction;
        private InputAction _dropAction;
        
        private IInteractable _interactable;
        private IPullable _pullable;
        private Arm _arm;
        private bool _isHolding;

        private void Awake()
        {
            _playerInputActions = new PlayerInputActions();
            _shootAction = _playerInputActions.Player.Fire;
            _shootAction.Enable();
            _shootAction.performed += ctx => Shoot();
            _shootAction.canceled += ctx => Release();
            _pullAction = _playerInputActions.Player.Pull;
            _pullAction.Enable();
            
        }

        private void Shoot()
        {
            if (_arm != null)
                return;

            // shoot ray from camera
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                _interactable = hit.collider.GetComponent<IInteractable>();
                if (_interactable == null)
                    return;
                IGrabable grabable = hit.collider.GetComponent<IGrabable>();
                if (grabable != null)
                {
                    _isHolding = true;
                    _arm = Instantiate(_armPrefab, _armSpawnPosition.position, Quaternion.identity);
                    _arm.GrabObject(_interactable.GetGrabPoint(), _armHoldPosition, grabable);
                    return;
                }
                _pullable = hit.collider.GetComponent<IPullable>();
                LatchArm(_interactable);
            }
        }

        private void Release()
        {
            if (_arm == null)
                return;
            _arm.Release(_armSpawnPosition);
            _pullable = null;
        }

        private void LatchArm(IInteractable latchObj)
        {
            _arm = Instantiate(_armPrefab, _armSpawnPosition.position, Quaternion.identity);
            _arm.Interact(latchObj);
        }

        private void Update()
        {
            if (!_pullAction.IsPressed() || !_shootAction.IsPressed())
                return;
            
            _pullable?.Pull();
        }
    }
}