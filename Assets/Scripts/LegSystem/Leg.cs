using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace LegSystem
{
    public class Leg : MonoBehaviour
    {
        public static Leg Instance;
        [SerializeField] private AnimationCurve _legReturnCurve;
        [SerializeField] private AnimationCurve _legReturnRotationCurve;
        [SerializeField] private float _legReturnSpeed;

        private void Awake()
        {
            Instance = this;
        }

        public void StartLegReturn(Transform origin, UnityAction onReturn = null)
        {
            StartCoroutine(ReturnLeg(origin, onReturn));
        }

        private IEnumerator ReturnLeg(Transform origin, UnityAction onReturn = null)
        {
            // lerp to position
            float t = 0;
            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;
            while (t < 1)
            {
                t += _legReturnSpeed * Time.deltaTime;
                float a = _legReturnCurve.Evaluate(t);
                float b = _legReturnRotationCurve.Evaluate(t);
                transform.position = Vector3.Lerp(startPos, origin.position, a);
                transform.rotation = Quaternion.Lerp(startRot, origin.rotation, b);
                yield return null;
            }
            onReturn?.Invoke();
            Destroy(gameObject);
        }
    }
}