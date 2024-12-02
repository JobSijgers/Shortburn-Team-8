using UnityEditor;
using UnityEngine;

namespace LimbsPickup
{
    public class LimbPickup : MonoBehaviour
    {
        public MonoScript LimbScript;
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
            Destroy(gameObject);
        }
    }
}