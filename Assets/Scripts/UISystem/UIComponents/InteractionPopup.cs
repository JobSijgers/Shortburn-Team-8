using HandScripts.Core;
using HandScripts.Pull;
using HandScripts.Use;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UISystem.UIComponents
{
    public class InteractionPopup : MonoBehaviour
    {
        private TMP_Text _text;
        private CameraController _camera;
        private PlayerInputActions _inputActions;
        [SerializeField] private float _minDistance;

        private void Start()
        {
            _inputActions = new PlayerInputActions();
            _text = GetComponent<TMP_Text>();
            _camera = CameraController.Instance;
        }

        private float GetDistanceToPlayer(Vector3 position) => Vector3.Distance(_camera.transform.position, position);
        private void Update()
        {
            var lookAtObject = _camera.GetLookAtObject();
    
            if (!lookAtObject.TryGetComponent(out IHandInteractable interactable) || !interactable.CurrentlyInteractable())
            {
                _text.text = string.Empty;
                return;
            }

            var objectTransform = interactable.GetObjectTransform();
    
            if (GetDistanceToPlayer(objectTransform.position) > _minDistance)
            {
                _text.text = string.Empty;
                return;
            }

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

            return null;
        }
        private string GetInteractionKey(InputAction action)
        {
            // get binding path
            string path = action.bindings[0].path;
            int lastSlash = path.LastIndexOf('/');
            if (lastSlash >= 0 && lastSlash < path.Length - 1)
            {
                return path.Substring(lastSlash + 1);
            }
            return path;
        }
    }
}