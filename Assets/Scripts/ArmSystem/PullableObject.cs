using PathCreation;
using UnityEngine;
using UnityEngine.Events;

namespace ArmSystem
{
    public class PullableObject : MonoBehaviour, IInteractable, IPullable
    {
        [SerializeField] private UnityEvent _onPathCompleted;
        [SerializeField] private Transform _grabPoint;
        [SerializeField] private PathCreator _pullPath;
        [SerializeField] private float _grabSpeed;
        public Transform GetGrabPoint() => _grabPoint;

        private bool _pathCompleted;
        private float _distance;


        private void Start()
        {
            transform.position = _pullPath.path.GetPointAtDistance(0, EndOfPathInstruction.Stop);
            transform.rotation = _pullPath.path.GetRotationAtDistance(0, EndOfPathInstruction.Stop);
        }

        public void Interacted()
        {
        }

        public void Pull()
        {
            if (_pathCompleted) 
                return;

            _distance += _grabSpeed * Time.deltaTime;
            transform.position = _pullPath.path.GetPointAtDistance(_distance, EndOfPathInstruction.Stop);
            transform.rotation = _pullPath.path.GetRotationAtDistance(_distance, EndOfPathInstruction.Stop);

            if (transform.position !=
                _pullPath.path.GetPointAtDistance(_pullPath.path.length, EndOfPathInstruction.Stop)) return;
            
            _onPathCompleted.Invoke();
            _pathCompleted = true;
        }
    }
}