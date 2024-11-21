using UnityEngine;

namespace ArmSystem
{
    public class GrabableObject : MonoBehaviour, IGrabable, IInteractable
    {
        [SerializeField] private Transform _grabPoint;
        public Transform GetGrabPoint() => _grabPoint;

        public void Interacted()
        {
            
        }

        public void Grabbed(Transform newParent)
        {
            transform.SetParent(newParent);
        }

        public void Drop()
        {
            transform.SetParent(null);
            
        }
    }
}