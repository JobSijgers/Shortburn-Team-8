using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Component_movement
{
    public class GenericMovement : MonoBehaviour
    {
        [SerializeField] private float _movementSpeed;
        [SerializeField] private AnimationCurve _movementCurve;
        [SerializeField] private float _delay;
        [SerializeField] private Vector3 _endPos;
        [SerializeField] private bool _useLocalPos;
        private Vector3 _startPos;

        public void StartMovement()
        {
            StartCoroutine(Move());
        }

        private IEnumerator Move()
        {
            yield return new WaitForSeconds(_delay);
            // lerp to end pos
            _startPos = _useLocalPos ? transform.localPosition : transform.position;
            float t = 0;
            while (t < 1)
            {
                float curveValue = _movementCurve.Evaluate(t);
                t += Time.deltaTime * _movementSpeed;
                if (_useLocalPos)
                    transform.localPosition = Vector3.Lerp(_startPos, _endPos, curveValue);
                else
                    transform.position = Vector3.Lerp(_startPos, _endPos, curveValue);
                yield return null;
            }
        }
    }
}
