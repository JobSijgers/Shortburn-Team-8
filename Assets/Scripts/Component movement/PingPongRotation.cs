using System.Collections;
using UnityEngine;

namespace Component_movement
{
    public class PingPongRotation : MonoBehaviour
    {
        [SerializeField] private float _movementSpeed;
        [SerializeField] private Quaternion _endRotation;

        private Quaternion _startRotation;
        private bool _isMoving;
        private float _t;
        private Coroutine _moveRoutine;

    
        private void Start()
        {
            _startRotation = transform.rotation;
        }

        public void RotateForward()
        {
            if (_moveRoutine != null) StopCoroutine(_moveRoutine);
            _moveRoutine = StartCoroutine(MoveRoutine(_endRotation));
        }

        public void RotateBack()
        {
            if (_moveRoutine != null) StopCoroutine(_moveRoutine);
            _moveRoutine = StartCoroutine(MoveRoutine(_startRotation));

        }

        private IEnumerator MoveRoutine(Quaternion endRotation)
        {
            while (transform.rotation != endRotation)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, endRotation, _movementSpeed * Time.deltaTime);
                yield return null;
            }

            _moveRoutine = null;
        }
    }
}