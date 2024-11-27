using System;
using HandScripts.Core;
using PathCreation;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace HandScripts.Pull
{
    public class PullableObject : MonoBehaviour, IHandInteractable, IHandPullable
    {
        [SerializeField] private Transform _handHoldPoint;
        [SerializeField] private float _pullSpeed;
        [SerializeField] private PathCreator _pathCreator;
        [Range(-360, 360)] [SerializeField] private float _minAngle;
        [Range(-360, 360)] [SerializeField] private float _maxAngle;

        private float _distanceTravelled;


        public Transform GetHeldPoint() => _handHoldPoint;
        public EInteractType GetInteractType() => EInteractType.Pull;
        public Transform GetObjectTransform() => transform;
        public bool HasBeenPulled() => _distanceTravelled >= _pathCreator.path.length;

        private void Start()
        {
            SetPositionAndRotationAtPathDistance(0);
        }


        public void Pull(UnityAction onComplete)
        {
            _distanceTravelled += _pullSpeed * Time.deltaTime;
            SetPositionAndRotationAtPathDistance(_distanceTravelled);

            if (_distanceTravelled >= _pathCreator.path.length)
            {
                onComplete?.Invoke();
            }
        }

        public bool CanPull(Vector3 playerPosition)
        {
            //if the player is within the min and max angle of the object
            Vector3 directionToPlayer = playerPosition - transform.position;
            float angle = Vector3.SignedAngle(transform.forward, directionToPlayer, Vector3.up);
            return angle >= _minAngle && angle <= _maxAngle;
        }

        private void SetPositionAndRotationAtPathDistance(float distance)
        {
            transform.position = _pathCreator.path.GetPointAtDistance(distance, EndOfPathInstruction.Stop);
            transform.rotation = _pathCreator.path.GetRotationAtDistance(distance, EndOfPathInstruction.Stop);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_minAngle > _maxAngle)
            {
                _maxAngle = _minAngle;
            }
        }

        private void OnDrawGizmosSelected()
        {
            //draw the lines to show the min and max angle
            Vector3 minAnglePosition = transform.position + Quaternion.Euler(0, _minAngle, 0) * transform.forward * 10;
            Vector3 maxAnglePosition = transform.position + Quaternion.Euler(0, _maxAngle, 0) * transform.forward * 10;
            Gizmos.DrawLine(transform.position, minAnglePosition);
            Gizmos.DrawLine(transform.position, maxAnglePosition);
            //fill the area between the min and max angle
            Handles.color = new Color(1, 0, 0, 0.1f);
            Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, _minAngle, 10);
            Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, _maxAngle, 10);
        }
#endif
    }
}