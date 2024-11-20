using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace ArmSystem
{
    public class Lever : MonoBehaviour, IGrabbable
    {
        [SerializeField] private Transform _grabPoint;
        [SerializeField] private UnityEvent _onGrabbed;
        
        public Transform GetGrabPoint() => _grabPoint;

        public void Latched()
        {
            _onGrabbed.Invoke();
        }
    }
}
