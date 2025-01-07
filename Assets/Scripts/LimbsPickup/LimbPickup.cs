using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace LimbsPickup
{
    public class LimbPickup : MonoBehaviour
    {
        public MonoScript LimbScript;
        [SerializeField] private UnityEvent _onPickup;
        private LimbsController _limbsController;
        private void Start()
        {
            _limbsController = LimbsController.Instance;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;
            _limbsController.PickupLimb(LimbScript);
            _onPickup?.Invoke();
            Destroy(gameObject);
        }
    }
}