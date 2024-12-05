using UnityEngine;
using System;
using System.Collections;
using HandScripts.Grab;
using PathCreation.Examples;
using UnityEngine.Events;

namespace HandScripts.ProceduralAnimation
{
    [Serializable]
    public class Finger
    {
        public string Name;
        public Transform Target;
        public Vector3 StartPosition;
    }
    public class ProceduralHandAnimation : MonoBehaviour
    {
        [SerializeField] private Finger[] _fingers;
        [SerializeField] private float _animationSpeed = 1;
        [SerializeField] private float _fingerSpeed = 1;
        [SerializeField] [Range(0, 1)]private float _fingerAnimStartPrc;

        private GrabPoint _grabPoint;
        private PathFollower _pathFollower;

        private void Start()
        {
            _pathFollower = GetComponent<PathFollower>();
            foreach (var finger in _fingers)
            {
                finger.StartPosition = finger.Target.localPosition;
            }
        }

        public Finger GetFinger(string name) => Array.Find(_fingers, finger => finger.Name == name);
        public PathFollower GetPathFollower() => _pathFollower;
        public void SetGrabPoint(GrabPoint grabPoint) => _grabPoint = grabPoint;
        public void SetFingerPosition(string name, Vector3 position)
        {
            Finger finger = GetFinger(name);
            if (finger != null) finger.Target.position = position;
        }
        
        public void MoveToGrabPoint(UnityAction onComplete = null)
        {
            StartCoroutine(AnimateHand(_grabPoint, onComplete));
        }
        public void ResetFingers()
        {
            foreach (Finger finger in _fingers)
            {
                StartCoroutine(AnimateFinger(finger.StartPosition, finger, true));
            }
        }

        private IEnumerator AnimateHand(GrabPoint point, UnityAction onComplete = null)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = point.transform.position;
            Quaternion startRot = transform.rotation;
            Quaternion endRot = point.transform.rotation;
            float t = 0;
            bool moveFingers = true;
            while (t < 1)
            {
                t += Time.deltaTime * _animationSpeed;
                transform.position = Vector3.Slerp(startPos, endPos, t);
                transform.rotation = Quaternion.Slerp(startRot, endRot, t);
                if (t >= _fingerAnimStartPrc && moveFingers)
                {
                    moveFingers = false;
                    foreach (Finger finger in _fingers)
                    {
                        StartCoroutine(AnimateFinger(point.GetFingerPosition(finger.Name), finger));
                    }
                }
                yield return null;
            }

            float dynamicWaitTime = 1.2f / _fingerSpeed;
            yield return new WaitForSeconds(dynamicWaitTime);
            onComplete?.Invoke();
        }
        private IEnumerator AnimateFinger(Vector3 target, Finger finger, bool isLocal = false)
        {
            Vector3 startPos = finger.Target.position;
            Vector3 endPos = target;
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * _fingerSpeed;
                if (isLocal) finger.Target.localPosition = Vector3.Slerp(startPos, endPos, t);
                else finger.Target.position = Vector3.Slerp(startPos, endPos, t);
                yield return null;
            }
        }
    }
}

