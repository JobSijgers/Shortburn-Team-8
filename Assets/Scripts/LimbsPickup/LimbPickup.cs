using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace LimbsPickup
{
    public class LimbPickup : MonoBehaviour
    {
        [SerializeField] private string limbClassName; // Name of the class
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

            // Use reflection to get the Type from the class name
            Type limbType = Type.GetType(limbClassName);
            if (limbType == null)
            {
                Debug.LogError($"Limb class '{limbClassName}' not found!");
                return;
            }

            _limbsController.PickupLimb(limbType);
            _onPickup?.Invoke();
            Destroy(gameObject);
        }
    }
}