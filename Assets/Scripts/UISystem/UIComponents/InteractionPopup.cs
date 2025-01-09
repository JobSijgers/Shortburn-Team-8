using HandScripts.Core;
using HandScripts.Pull;
using HandScripts.Use;
using Player;
using TMPro;
using UnityEngine;

namespace UISystem.UIComponents
{
    public class InteractionPopup : MonoBehaviour
    {
        private TMP_Text _text;
        private CameraController _camera;

        private void Start()
        {
            _text = GetComponent<TMP_Text>();
            _camera = CameraController.Instance;
        }

        private void Update()
        {
            if (_camera.GetLookAtObject().TryGetComponent(out IHandInteractable interactable))
            {
                if (interactable.GetObjectTransform().TryGetComponent(out IHandUseable useableObject))
                {
                    if (useableObject.HasBeenUsed())
                    {
                        _text.text = string.Empty;
                        return;
                    }
                }

                if (interactable.GetObjectTransform().TryGetComponent(out IHandPullable pullableObject))
                {
                    if (pullableObject.HasBeenPulled())
                    {
                        _text.text = string.Empty;
                        return;
                    }
                }
                _text.text = "Press LMB to interact";
            }
            else
            {
                _text.text = string.Empty;
            }
        }
    }
}