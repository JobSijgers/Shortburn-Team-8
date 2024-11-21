using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace ArmSystem
{
    public class InteractableObject : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform _grabPoint;
        [SerializeField] private UnityEvent _onInteract;
        
        public Transform GetGrabPoint() => _grabPoint;

        public void Interacted()
        {
            _onInteract.Invoke();
        }
    }
}
