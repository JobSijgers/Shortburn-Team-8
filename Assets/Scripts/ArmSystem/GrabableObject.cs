using UnityEngine;

namespace ArmSystem
{
    public class GrabableObject : MonoBehaviour, IGrabable, IInteractable
    {
        [SerializeField] private Transform _grabPoint;
        [SerializeField] private string _key;
        public Transform GetGrabPoint() => _grabPoint;

        public void Interacted()
        {
        }

        public void Grabbed(Transform newParent) => transform.SetParent(newParent);
        public void Drop() => transform.SetParent(null);
        public string GetGrabableKey() => _key;
    }
}