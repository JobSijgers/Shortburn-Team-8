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

    
        private void Start()
        {
            _startPos = transform.position;
        }

        public void MoveForward()
        {
            _isMoving = true;
        }

        public void MoveBack()
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
                Vector3 newPos = Vector3.Lerp(_startPos, _endPos, t);
                transform.position = newPos;
            }
            else
            {
                if (_t <= 0)
                    return;
                _t -= Time.deltaTime;
                float t = _moveCurve.Evaluate(_t);
                Vector3 newPos = Vector3.Lerp(_startPos, _endPos, t);
                transform.position = newPos;
            }
        }
    }
}