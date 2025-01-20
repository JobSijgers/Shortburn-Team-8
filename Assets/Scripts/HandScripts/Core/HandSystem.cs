using System;
using System.Collections;
using HandScripts.Grab;
using HandScripts.Pull;
using HandScripts.Use;
using Unity.Mathematics;
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
            Debug.Log(_handInUse);
            if (_handInUse)
                return;
            TryUseInteractable(ShootRay());
        }

        private void TryUseInteractable(RaycastHit hit)
        {
            Debug.Log(hit.collider.gameObject.name);
            // get closest interactable on object
            IHandInteractable[] interactables = hit.collider?.GetComponents<IHandInteractable>();
            float closestDistance = float.MaxValue;
            IHandInteractable interactable = null;
            foreach (IHandInteractable current in interactables)
            {
                GrabPoint grabPoint = current.GetGrabPoint();
                float distance = Vector3.Distance(transform.position, grabPoint.GrabPointTransform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    interactable = current;
                }
            }

            if (interactable == null)
                return;

            _rightHand.ShotArm(_rightHand.transform);
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
            _leftHand.MoveToPoint(_leftHandInactiveHolder, _leftHandInactiveHolder,
                () => { _leftHand.gameObject.SetActive(false); });

            _rightHand.ShotArm(_rightHand.transform);
            _rightHand.MoveToPoint(_leftHandInactiveHolder, null, () =>
            {
                IHandGrabable storedObject = _leftHand.GetStoredObject();
                storedObject.SetParent(_rightHand.GetStoragePoint().transform);
                storedObject.Released();
                storedObject.ResetPosition(quaternion.identity);
                
                // set finger positions
                IHandInteractable handInteractable = (IHandInteractable)storedObject;
                _rightHand.MoveFingersToGrabPoint(handInteractable.GetGrabPoint(), true);
                _rightHand.MoveToGrabPoint(interactable.GetGrabPoint(), null, () =>
                {
                    storedObject.SetParent(null);
                    IHandInteractable handInteractable = (IHandInteractable)storedObject;
                    handInteractable.GetObjectTransform().localRotation = quaternion.identity;
                    Debug.Log(_leftHand.GetStoredObject());
                    deposit.OnDeposit(_leftHand.GetStoredObject());
                    ReturnRightHand(true);
                    _leftHand.StoreObject(null);
                });
            });
        }

        private void HandleHandGrab(IHandInteractable interactable)
        {
            if (_leftHand.GetStoredObject() != null)
            {
                Debug.Log("Hand is already holding an object");
                return;
            }

            IHandGrabable grabable = (IHandGrabable)interactable;
            _handInUse = true;
            _rightHand.MoveToGrabPoint(interactable.GetGrabPoint(), null, () => ReturnGrabItem(grabable));
        }

        private void ReturnGrabItem(IHandGrabable grabable)
        {
            grabable.SetParent(_rightHand.transform);
            grabable.Grabbed();
            _rightHand.MoveToPoint(_leftHandInactiveHolder, null, () =>
            {
                IHandInteractable interactable = (IHandInteractable)grabable;
                Quaternion oldRot = interactable.GetObjectTransform().localRotation;
                grabable.SetParent(_leftHand.GetStoragePoint().transform);
                grabable.ResetPosition(oldRot);
                _leftHand.StoreObject(grabable);
                _leftHand.gameObject.SetActive(true);
                _leftHand.Grab(interactable.GetGrabPoint());
                _leftHand.MoveToPoint(_leftHandActiveHolder, _leftHandActiveHolder, null);
                ReturnRightHand(true);
            });
        }

        private void HandleHandPull(IHandInteractable interactable)
        {
            IHandPullable pullable = (IHandPullable)interactable;
            if (pullable.HasBeenPulled())
                return;
            _handInUse = true;
            _rightHand.MoveToGrabPoint(interactable.GetGrabPoint(), interactable.GetObjectTransform(),
                () => StartCoroutine(PullRoutine(pullable)));
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
            _rightHand.MoveToGrabPoint(interactable.GetGrabPoint(), interactable.GetObjectTransform(),
                () => useable.Use(() => ReturnRightHand(true)));
        }

        private void ReturnRightHand(bool resetFingers = false)
        {
            if (resetFingers)
            {
                _rightHand.ResetFingers();
            }

            _rightHand.ReturnArm(_rightHand.transform);
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