using UnityEngine;

namespace Component_movement
{
    public class PingPongRotation : MonoBehaviour
    {
        [SerializeField] private float _movementSpeed;
        [SerializeField] private Vector3 _endRotation;
        [SerializeField] private AnimationCurve _moveCurve;

        private Vector3 _startRotation;
        private bool _isMoving;
        private float _t;

    
        private void Start()
        {
            _startRotation = transform.eulerAngles;
        }

        public void RotateForward()
        {
            _isMoving = true;
        }

        public void RotateBack()
        {
            _isMoving = false;
        }
        private void Update()
        {
            if (_isMoving)
            {
                if (_t >= 1) 
                    return;
                _t += Time.deltaTime;
                float t = _moveCurve.Evaluate(_t);
                Quaternion newPos = Quaternion.Euler(Vector3.Lerp(_startRotation, _endRotation, t));
                transform.rotation = newPos;
            }
            else
            {
                if (_t <= 0)
                    return;
                _t -= Time.deltaTime;
                float t = _moveCurve.Evaluate(_t);
                Quaternion newPos = Quaternion.Euler(Vector3.Lerp(_startRotation, _endRotation, t));
                transform.rotation = newPos;
            }
        }
    }
}