using System.Collections;
using UnityEngine;

namespace Component_movement
{
    public class PingPongMovement : MonoBehaviour
    {
        [SerializeField] private float _movementSpeed;
        [SerializeField] private Vector3 _endPos;
        [SerializeField] private AnimationCurve _moveCurve;

        private Vector3 _startPos;
        private bool _isMoving;
        private float _t;
        private Coroutine _moveRoutine;
    
        private void Start()
        {
            _startPos = transform.position;
        }

        public void MoveForward()
        {
            if (_moveRoutine != null) StopCoroutine(_moveRoutine);
            _moveRoutine = StartCoroutine(MoveRoutine(_endPos));
        }

        public void MoveBack()
        {
            if (_moveRoutine != null) StopCoroutine(_moveRoutine);
            _moveRoutine = StartCoroutine(MoveRoutine(_startPos));
        }

        private IEnumerator MoveRoutine(Vector3 targetPosition)
        {
            while (transform.position != targetPosition)
            {
                float t = _moveCurve.Evaluate(Vector3.Distance(transform.position, targetPosition) / Vector3.Distance(_startPos, _endPos));
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, _movementSpeed * Time.deltaTime * t);
                yield return null;
            }

            _moveRoutine = null;
        }
    }
}