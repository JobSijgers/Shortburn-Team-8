using System;
using HandScripts.Core;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;

namespace HandScripts.Grab
{
    public class GrabableObject : MonoBehaviour, IHandInteractable, IHandGrabable
    {
        [SerializeField] private GrabPoint _heldPoint;
        [SerializeField] private string _depositKey;
        [SerializeField] private bool _currentlyInteractable = true;
        private LayerMask _defaultLayer;

        private void Start()
        {
            _defaultLayer = gameObject.layer;
        }

        public GrabPoint GetGrabPoint() => _heldPoint;
        public EInteractType GetInteractType() => EInteractType.Grab;
        public Transform GetObjectTransform() => transform;
        public void SetParent(Transform newParent) => transform.SetParent(newParent);
        public void Grabbed() => gameObject.layer = LayerMask.NameToLayer("GrabbedObject");
        public string GetDepositKey() => _depositKey;
        public void Released() => gameObject.layer = _defaultLayer;
        public bool CurrentlyInteractable() => _currentlyInteractable;

        public void ResetPosition(Quaternion localRotation)
        {
            transform.localRotation = localRotation;
            transform.localPosition = Vector3.zero;
        }

    }
}