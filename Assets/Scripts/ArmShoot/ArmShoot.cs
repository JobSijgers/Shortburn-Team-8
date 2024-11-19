using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArmShoot : MonoBehaviour
{
    [SerializeField] private GameObject _armPrefab;
    [SerializeField] private Transform _armPos;
    private PlayerInputActions _playerInputActions;
    private InputAction _shootAction;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _shootAction = _playerInputActions.Player.Fire;
        _shootAction.Enable();
        _shootAction.performed += ctx => Shoot();
    }

    private void Shoot()
    {
        // check if arm exists
        if (Arm.Instance != null) return;
        
        // shoot ray fromc camera
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                ILatchable latchObj = hit.collider.GetComponent<ILatchable>();
                if (latchObj == null) return;
                StartCoroutine(LatchArm(latchObj));
                latchObj.Latched();
            }
        }
    }

    private IEnumerator LatchArm(ILatchable latchObj)
    {
        GameObject armObj = Instantiate(_armPrefab, _armPos.position, Quaternion.identity);
        Arm arm = armObj.GetComponent<Arm>();
        yield return StartCoroutine(arm.Latch(latchObj));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(arm.Release(_armPos));
    }
}