using System;
using HandScripts.Core;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace HandScripts.Grab
{
    public class GrabableObject : MonoBehaviour, IHandInteractable, IHandGrabable
    {
        [SerializeField] private Transform _heldPoint;
        [SerializeField] private string _depositKey;
        private LayerMask _defaultLayer;

        private void Start()
        {
            _defaultLayer = gameObject.layer;
        }

        public Transform GetHeldPoint() => _heldPoint;
        public EInteractType GetInteractType() => EInteractType.Grab;
        public Transform GetObjectTransform() => transform;
        public void SetParent(Transform newParent) => transform.SetParent(newParent);
        public void Grabbed() => gameObject.layer = LayerMask.NameToLayer("GrabbedObject");
        public string GetDepositKey() => _depositKey;

        public void Released() => gameObject.layer = _defaultLayer;

        public void ResetPosition()
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }
}