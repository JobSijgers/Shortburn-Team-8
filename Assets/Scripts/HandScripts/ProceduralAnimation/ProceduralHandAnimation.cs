using UnityEngine;
using System;
using System.Collections;
using HandScripts.Grab;

namespace HandScripts.ProceduralAnimation
{
    [Serializable]
    public class Finger
    {
        public string Name;
        public Transform Target;
    }
    public class ProceduralHandAnimation : MonoBehaviour
    {
        [SerializeField] private Finger[] _fingers;
        [SerializeField] private float _animationSpeed = 1;
        [SerializeField] private GrabPoint _grabPoint;
        private void Start()
        {
            StartAnimation(_grabPoint);
        }
        public Finger GetFinger(string name) => Array.Find(_fingers, finger => finger.Name == name);
        public void SetFingerPosition(string name, Vector3 position)
        {
            Finger finger = GetFinger(name);
            if (finger != null) finger.Target.position = position;
        }
        public void StartAnimation(GrabPoint grabPoint)
        {
            StartCoroutine(AnimateHand(grabPoint));
        }

        private IEnumerator AnimateHand(GrabPoint point)
        {
            Vector3 startPosition = transform.position;
            Vector3 endPosition = point.transform.position;
            Quaternion startRotation = transform.rotation;
            Quaternion endRotation = point.transform.rotation;
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * _animationSpeed;
                transform.position = Vector3.Lerp(startPosition, endPosition, t);
                transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
                yield return null;
            }
            foreach (Finger finger in _fingers)
            {
                StartCoroutine(AnimateFinger(point, finger));
            }
        }
        private IEnumerator AnimateFinger(GrabPoint grabPoint, Finger finger)
        {
            Vector3 StartPos = finger.Target.position;
            Vector3 EndPos = grabPoint.GetFingerPosition(finger.Name);
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * _animationSpeed;
                finger.Target.position = Vector3.Lerp(StartPos, EndPos, t);
                yield return null;
            }
        }
    }
}

