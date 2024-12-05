using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Component_movement
{
    public class GenericRotation : MonoBehaviour
    {
        [SerializeField] private float _movementSpeed;
        [SerializeField] private AnimationCurve _movementCurve;
        [SerializeField] private Quaternion _endRot;
        [SerializeField] private float _delay;
        private Quaternion _startRot;

        public void StartMovement()
        {
            StartCoroutine(Move());
        }

        private IEnumerator Move()
        {
            yield return new WaitForSeconds(_delay);
            // lerp to end pos
            _startRot = transform.localRotation;
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / _movementSpeed;
                float curveValue = _movementCurve.Evaluate(t);
                transform.localRotation = Quaternion.Lerp(_startRot, _endRot, curveValue);
                yield return null;
            }
        }
    }
}
