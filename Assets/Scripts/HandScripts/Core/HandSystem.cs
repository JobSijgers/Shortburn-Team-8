using System;
using System.Collections;
using HandScripts.Pull;
using HandScripts.Use;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace HandScripts.Core
{
    public class HandSystem : MonoBehaviour
    {
        [Header("Hand Settings")]
        [SerializeField] private Hand _rightHand;
        [SerializeField] private Transform _rightHandHolder;

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
        }

        private void TryUseHand()
        {
            if (_handInUse)
                return;
            IHandInteractable interactable = ShootRay().collider?.GetComponent<IHandInteractable>();

            if (interactable == null)
                return;

            switch (interactable.GetInteractType())
            {
                case EInteractType.Grab:
                    break;
                case EInteractType.Pull:
                    HandleHandPull(interactable);
                    break;
                case EInteractType.Use:
                    HandleHandUse(interactable);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleHandPull(IHandInteractable interactable)
        {
            IHandPullable pullable = (IHandPullable)interactable;
            _handInUse = true;
            _rightHand.MoveToPoint(interactable.GetHeldPoint(), interactable.GetObjectTransform(), () => StartCoroutine(PullRoutine(pullable)));
        }

        private IEnumerator PullRoutine(IHandPullable pullable)
        {
            bool shouldReturn = false;
            
            while (shouldReturn == false)
            {
                if (_pullAction.IsPressed())
                {
                    pullable.Pull(() => shouldReturn = true);
                }

                if (!_useAction.IsPressed() && !_pullAction.IsPressed())
                {
                    shouldReturn = true;
                }

                yield return null;
            }
            
            ReturnHand();
        }
        private void HandleHandUse(IHandInteractable interactable)
        {
            IHandUseable useable = (IHandUseable)interactable;
            if (useable.HasBeenUsed())
                return;
            _handInUse = true;
            _rightHand.MoveToPoint(interactable.GetHeldPoint(), interactable.GetObjectTransform(),
                () => useable.Use(ReturnHand));
        }

        private void ReturnHand()
        {
            _rightHand.MoveToPoint(_rightHandHolder, _rightHandHolder, OnHandUseComplete);
        }

        private void OnHandUseComplete()
        {
            _rightHand.transform.position = _rightHandHolder.position;
            _rightHand.transform.rotation = _rightHandHolder.rotation;
            _rightHand.transform.SetParent(_rightHandHolder);
            _handInUse = false;
        }

        private RaycastHit ShootRay()
        {
            Physics.Raycast(_rayOrigin.position, _rayOrigin.forward, out RaycastHit hit, _rayDistance, _layerMask);
            return hit;
        }
    }
}