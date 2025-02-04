using System;
using HandScripts.Core;
using HandScripts.Grab;
using PathCreation;
using Player;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace HandScripts.Pull
{
    public class PullableObject : MonoBehaviour, IHandInteractable, IHandPullable
    {
        [SerializeField] private GrabPoint _handHoldPoint;
        [SerializeField] private float _pullSpeed;
        [SerializeField] private Transform _destination;
        [SerializeField] private bool _updateY;
        [SerializeField] private bool _updateRotation;
        [Range(-360, 360)] [SerializeField] private float _minAngle;
        [Range(-360, 360)] [SerializeField] private float _maxAngle;
        [SerializeField] private Vector3 _angleDirection;
        [SerializeField] private UnityEvent _onPullComplete;
        [SerializeField] private UnityEvent<float> _onPullUpdate;
        [SerializeField] private bool _currentlyInteractable = true;
        [SerializeField] private bool _autoSetStartPosition = true;
        [SerializeField] private Vector3 _startPosition;

        private float _distanceTravelled;
        private float _totalDistance;
        private bool _resetStartPos;

        public GrabPoint GetGrabPoint() => _handHoldPoint;
        public EInteractType GetInteractType() => EInteractType.Pull;
        public Transform GetObjectTransform() => transform;
        public bool HasBeenPulled() => Vector3.Distance(transform.position, _destination.position) < 0.1f;
        public bool CurrentlyInteractable() => _currentlyInteractable;

        private void Start()
        {
            if (_autoSetStartPosition)  
            {
                _startPosition = transform.position;
            }
            _totalDistance = Vector3.Distance(_startPosition, _destination.position);
            SetPositionAndRotationAtPathDistance();
        }


        public void Pull(UnityAction onComplete)
        {
            SetPositionAndRotationAtPathDistance();

            float distance = Vector3.Distance(transform.position, _destination.position);
            float t = 1 - distance / _totalDistance;
            _onPullUpdate?.Invoke(t);

            if (Vector3.Distance(transform.position, _destination.position) > 0.01f)
                return;
            onComplete?.Invoke();
            _onPullComplete?.Invoke();
        }

        public bool CanPull(Vector3 playerPosition)
        {
            //if the player is within the min and max angle of the object
            Vector3 directionToPlayer = playerPosition - transform.position;
            float angle = Vector3.SignedAngle(_angleDirection, directionToPlayer, Vector3.up);
            return angle >= _minAngle && angle <= _maxAngle;
        }

        private void SetPositionAndRotationAtPathDistance()
        {
            Vector3 pos = Vector3.MoveTowards(transform.position, _destination.position, _pullSpeed * Time.deltaTime);
            pos.y = _updateY ? pos.y : transform.position.y;
            transform.position = pos;

            Quaternion rot =
                Quaternion.RotateTowards(transform.rotation, _destination.rotation, _pullSpeed * Time.deltaTime);
            if (_updateRotation) transform.rotation = rot;
        }

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            //draw the lines to show the min and max angle
            Vector3 minAnglePosition = transform.position + Quaternion.Euler(0, _minAngle, 0) * _angleDirection * 10;
            Vector3 maxAnglePosition = transform.position + Quaternion.Euler(0, _maxAngle, 0) * _angleDirection * 10;
            Gizmos.DrawLine(transform.position, minAnglePosition);
            Gizmos.DrawLine(transform.position, maxAnglePosition);
            //fill the area between the min and max 
            PlayerMovement player = FindObjectOfType<PlayerMovement>();
            if (player != null)
            {
                if (CanPull(player.transform.position))
                {
                    Handles.color = new Color(0, 2, 0, 0.1f);
                }

                else
                {
                    Handles.color = new Color(1, 0, 0, 0.1f);
                }
            }
            else
            {
                Handles.color = new Color(1, 0, 0, 0.1f);
            }
            Handles.DrawSolidArc(transform.position, Vector3.up, _angleDirection, _minAngle, 10);
            Handles.DrawSolidArc(transform.position, Vector3.up, _angleDirection, _maxAngle, 10);
        }
#endif
    }
}