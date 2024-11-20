using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace ArmSystem
{
    public class ArmShoot : MonoBehaviour
    {
        [SerializeField] private Arm _armPrefab;
        [SerializeField] private Transform _armSpawnPosition;
        private PlayerInputActions _playerInputActions;
        private InputAction _shootAction;
        private InputAction _pullAction;

        private IGrabbable _grabbable;
        private IPullable _pullable;
        private Arm _arm;

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
            // check if arm exists
            if (Arm.Instance != null) return;

            // shoot ray fromc camera

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                _grabbable = hit.collider.GetComponent<IGrabbable>();
                if (_grabbable == null)
                    return;
                _pullable = hit.collider.GetComponent<IPullable>();
                StartCoroutine(LatchArm(_grabbable));
            }
        }

        private void Release()
        {
            if (_arm == null)
                return;
            StartCoroutine(_arm.Release(_armSpawnPosition));
        }

        private IEnumerator LatchArm(IGrabbable latchObj)
        {
            _arm = Instantiate(_armPrefab, _armSpawnPosition.position, Quaternion.identity);
            yield return StartCoroutine(_arm.Latch(latchObj));
            latchObj.Latched();
        }

        private void Update()
        {
            if (!_pullAction.IsPressed())
                return;

            _pullable?.Pull();
        }
    }
}