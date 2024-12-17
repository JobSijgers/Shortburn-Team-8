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
        private Vector3 _startPos;

        public void StartMovement()
        {
            StartCoroutine(Move());
        }

        private IEnumerator Move()
        {
            yield return new WaitForSeconds(_delay);
            // lerp to end pos
            _startPos = transform.position;
            float t = 0;
            while (t < 1)
            {
                float curveValue = _movementCurve.Evaluate(t);
                t += Time.deltaTime * _movementSpeed;
                transform.position = Vector3.Lerp(_startPos, _endPos, curveValue);
                yield return null;
            }
        }
    }
}
