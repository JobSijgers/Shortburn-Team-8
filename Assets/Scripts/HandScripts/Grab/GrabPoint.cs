using UnityEngine;
using System;
using PathCreation;

namespace HandScripts.Grab
{
    public class GrabPoint : MonoBehaviour
    {
        [SerializeField] private ProceduralAnimation.Finger[] _fingers;
        [SerializeField] private Transform _grabPoint;
        [SerializeField] private PathCreator _pathCreator;
        [SerializeField] private bool _usePathRotation;
        
        public Transform GrabPointTransform => _grabPoint;
        public PathCreator PathCreator => _pathCreator;
        public bool UsePathRotation => _usePathRotation;
        public Vector3 GetFingerPosition(string name) => Array.Find(_fingers, finger => finger.Name == name).Target.position; 
        public Transform GetFingerTransform(string name) => Array.Find(_fingers, finger => finger.Name == name).Target;
    }
}


