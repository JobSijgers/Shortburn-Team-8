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
        [SerializeField] private Quaternion _startRot;
        [SerializeField] private bool _autoSetStartRot = true;

        private void Start()
        {
            if (_autoSetStartRot)
                _startRot = transform.localRotation; 
        }

        public void StartMovement()
        {
            StartCoroutine(Move());
        }
        
        public void UpdateRotation(float t)
        {
            float curveValue = _movementCurve.Evaluate(t);
            transform.localRotation = Quaternion.Lerp(_startRot, _endRot, curveValue);
        }
        
        private IEnumerator Move()
        {
            yield return new WaitForSeconds(_delay);
            // lerp to end pos
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
