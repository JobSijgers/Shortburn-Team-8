using System;
using System.Collections;
using HandScripts.Grab;
using HandScripts.Pull;
using HandScripts.Use;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HandScripts.Core
{
    public class HandSystem : MonoBehaviour
    {
        [Header("Hand Settings")]
        [SerializeField] private Hand _rightHand;

        [SerializeField] private Transform _rightHandHolder;

        [Space(10)]
        [SerializeField] private StorageHand _leftHand;

        [SerializeField] private Transform _leftHandActiveHolder;
        [SerializeField] private Transform _leftHandInactiveHolder;

        [Header("Raycast Settings")]
        [SerializeField] private Transform _rayOrigin;

        [SerializeField] private float _rayDistance = 10f;
        [SerializeField] private LayerMask _layerMask;

        private bool _handInUse;
        private bool _handPulling;
        private PlayerInputActions _inputActions;
        private InputAction _useAction;
        private InputAction _pullAction;

        private void OnEnable()
        {
            _inputActions = new PlayerInputActions();
            _useAction = _inputActions.Hand.UseHand;
            _useAction.Enable();
            _useAction.performed += ctx => TryUseHand();

            _pullAction = _inputActions.Hand.Pull;
            _pullAction.Enable();
        }

        private void Start()
        {
            _rightHand.transform.position = _rightHandHolder.position;
            _rightHand.transform.rotation = _rightHandHolder.rotation;
            _rightHand.transform.SetParent(_rightHandHolder);

            _leftHand.transform.position = _leftHandInactiveHolder.position;
            _leftHand.transform.rotation = _leftHandInactiveHolder.rotation;
            _leftHand.transform.SetParent(_leftHandInactiveHolder);
            _leftHand.gameObject.SetActive(false);
        }

        private void TryUseHand()
        {
            if (_handInUse)
                return;
            TryUseInteractable(ShootRay());
        }

        private void TryUseInteractable(RaycastHit hit)
        {
            IHandInteractable interactable = hit.collider?.GetComponent<IHandInteractable>();

            if (interactable == null)
                return;

            switch (interactable.GetInteractType())
            {
                case EInteractType.Grab:
                    HandleHandGrab(interactable);
                    break;
                case EInteractType.Pull:
                    HandleHandPull(interactable);
                    break;
                case EInteractType.Use:
                    HandleHandUse(interactable);
                    break;
                case EInteractType.Deposit:
                    TryUseDeposit(interactable);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private RaycastHit ShootRay()
        {
            Physics.Raycast(_rayOrigin.position, _rayOrigin.forward, out RaycastHit hit, _rayDistance, _layerMask);
            return hit;
        }

        private void TryUseDeposit(IHandInteractable interactable)
        {
            if (_leftHand.GetStoredObject() == null)
                return;

            IDeposit deposit = (IDeposit)interactable;
            if (deposit.GetDepositKey() != _leftHand.GetStoredObject().GetDepositKey())
                return;

            _handInUse = true;
            _leftHand.MoveToPoint(_leftHandInactiveHolder, _leftHandInactiveHolder, () =>
            {
                _leftHand.gameObject.SetActive(false);
            });
                
            _rightHand.MoveToPoint(_leftHandInactiveHolder, null, () =>
            {
                IHandGrabable storedObject = _leftHand.GetStoredObject();
                storedObject.SetParent(_rightHand.GetStoragePoint().transform);
                storedObject.ResetPosition();
                _rightHand.MoveToPoint(interactable.GetGrabPoint().transform, null, () =>
                {
                    storedObject.SetParent(null);
                    
                    deposit.OnDeposit(_leftHand.GetStoredObject());
                    
                    ReturnRightHand(true);
                }, interactable.GetGrabPoint());
                _leftHand.StoreObject(null);
            });
        }

        private void HandleHandGrab(IHandInteractable interactable)
        {
            if (_leftHand.GetStoredObject() != null)
            {
                Debug.Log("Hand is already holding an object");
            }

            IHandGrabable grabable = (IHandGrabable)interactable;
            _handInUse = true;
            _rightHand.MoveToPoint(interactable.GetGrabPoint().transform, null, () => ReturnGrabItem(grabable), interactable.GetGrabPoint());
        }

        private void ReturnGrabItem(IHandGrabable grabable)
        {
            grabable.SetParent(_rightHand.transform);
            grabable.Grabbed();
            _rightHand.MoveToPoint(_leftHandInactiveHolder, null, () =>
            {
                grabable.SetParent(_leftHand.GetStoragePoint().transform);
                grabable.ResetPosition();
                _leftHand.StoreObject(grabable);
                _leftHand.gameObject.SetActive(true);
                _leftHand.MoveToPoint(_leftHandActiveHolder, _leftHandActiveHolder, null);
                ReturnRightHand();
            });
        }

        private void HandleHandPull(IHandInteractable interactable)
        {
            IHandPullable pullable = (IHandPullable)interactable;
            if (pullable.HasBeenPulled())
                return;
            _handInUse = true;
            _rightHand.MoveToPoint(interactable.GetGrabPoint().transform, interactable.GetObjectTransform(),
                () => StartCoroutine(PullRoutine(pullable)), interactable.GetGrabPoint());
        }

        private IEnumerator PullRoutine(IHandPullable pullable)
        {
            bool shouldReturn = false;

            while (shouldReturn == false)
            {
                if (_pullAction.IsPressed() && pullable.CanPull(transform.position))
                {
                    pullable.Pull(() => shouldReturn = true);
                }

                if (!_useAction.IsPressed() && !_pullAction.IsPressed())
                {
                    shouldReturn = true;
                }

                yield return null;
            }

            ReturnRightHand(true);
        }

        private void HandleHandUse(IHandInteractable interactable)
        {
            IHandUseable useable = (IHandUseable)interactable;
            if (useable.HasBeenUsed())
                return;
            _handInUse = true;
            _rightHand.MoveToPoint(interactable.GetGrabPoint().transform, interactable.GetObjectTransform(),
                () => useable.Use( () => ReturnRightHand(true)), interactable.GetGrabPoint());
        }

        private void ReturnRightHand(bool resetFingers = false)
        {
            if (resetFingers)
            {
                _rightHand.ResetFingers();
            }

            _rightHand.MoveToPoint(_rightHandHolder, _rightHandHolder, OnHandUseComplete);
        }

        private void OnHandUseComplete()
        {
            _rightHand.transform.position = _rightHandHolder.position;
            _rightHand.transform.rotation = _rightHandHolder.rotation;
            _rightHand.transform.SetParent(_rightHandHolder);
            _handInUse = false;
        }
    }
}