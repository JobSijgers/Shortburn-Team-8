using System;
using System.Collections.Generic;
using HandScripts.Core;
using HandScripts.Pull;
using HandScripts.Use;
using LimbsPickup;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UISystem.UIComponents
{
    [Serializable]
    public struct InputActionName
    {
        public string ActionName;
        public string InteractionMessage;
    }
    public class InteractionPopup : MonoBehaviour
    {
        [SerializeField] private float _minDistance;
        [SerializeField] private InputActionName[] _actionNames;
        private Dictionary<string, string> _inputActionNames = new();
        private TMP_Text _text;
        private CameraController _camera;
        private LimbsController _limbsController;
        private PlayerInputActions _inputActions;

        private void Start()
        {
            _inputActions = new PlayerInputActions();
            _text = GetComponent<TMP_Text>();
            _camera = CameraController.Instance;
            _limbsController = LimbsController.Instance;
            foreach (InputActionName name in _actionNames)
            {
                _inputActionNames.Add(name.ActionName, name.InteractionMessage);
            }
        }

        private float GetDistanceToPlayer(Vector3 position) => Vector3.Distance(_camera.transform.position, position);
        private bool ShouldPopup(Vector3 position) => GetDistanceToPlayer(position) < _minDistance && _limbsController.HandState;
        private void Update()
        {
            Transform lookAtObject = _camera.GetLookAtObject();
    
            if (!lookAtObject.TryGetComponent(out IHandInteractable interactable) || !interactable.CurrentlyInteractable())
            {
                _text.text = string.Empty;
                return;
            }

            Transform objectTransform = interactable.GetObjectTransform();
    
            if (!ShouldPopup(objectTransform.position))
            {
                _text.text = string.Empty;
                return;
            }
            Debug.Log(objectTransform.name);

            InputAction action = GetInteractionAction(objectTransform, out string interactionMessage);
    
            if (!string.IsNullOrEmpty(interactionMessage))
            {
                _text.text = interactionMessage;
            }
            else
            {
                _text.text = string.Empty;
            }
        }
        private InputAction GetInteractionAction(Transform objectTransform, out string message)
        {
            message = string.Empty;

            if (objectTransform.TryGetComponent(out IHandUseable useableObject))
            {
                if (useableObject.HasBeenUsed())
                {
                    return null;
                }
                message = $"Press {GetInteractionKey(_inputActions.Hand.UseHand)} to interact";
                return _inputActions.Hand.UseHand;
            }

            if (objectTransform.TryGetComponent(out IHandPullable pullableObject))
            {
                if (pullableObject.HasBeenPulled())
                {
                    return null;
                }
                message = $"Hold {GetInteractionKey(_inputActions.Hand.Pull)} to interact";
                return _inputActions.Hand.Pull;
            }

            message = $"Press {GetInteractionKey(_inputActions.Hand.UseHand)} to interact";
            return _inputActions.Hand.UseHand;
        }
        private string GetInteractionKey(InputAction action)
        {
            // get binding path
            string path = action.bindings[0].path;
            int lastSlash = path.LastIndexOf('/');
            if (lastSlash >= 0 && lastSlash < path.Length - 1)
            {
                string subString = path.Substring(lastSlash + 1);
                // search if binding is in dictionary
                if (_inputActionNames.TryGetValue(subString, out string interactionMessage))
                {
                    return interactionMessage;
                }
                return subString;
            }
            return path;
        }
    }
}