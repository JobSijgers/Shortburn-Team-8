using System;
using PathCreation;
using UnityEngine;
using UnityEngine.Serialization;

namespace ArmSystem
{
    public class PullableObject : MonoBehaviour, IGrabbable, IPullable
    {
        [SerializeField] private Transform _grabPoint;
        [SerializeField] private PathCreator _pullPath;
        [SerializeField] private float _grabSpeed;
        public Transform GetGrabPoint() => _grabPoint;

        private float _distance;
        

        private void Start()
        {
            transform.position = _pullPath.path.GetPointAtDistance(0, EndOfPathInstruction.Stop);
            transform.rotation = _pullPath.path.GetRotationAtDistance(0, EndOfPathInstruction.Stop);
        }

        public void Latched()
        {
            
        }
        
        public void Pull()
        {
            _distance += _grabSpeed * Time.deltaTime;
            transform.position = _pullPath.path.GetPointAtDistance(_distance, EndOfPathInstruction.Stop);
            transform.rotation = _pullPath.path.GetRotationAtDistance(_distance, EndOfPathInstruction.Stop);
        }
    }
}