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
        public void SetFingerPosition(string name, Vector3 position)
        {
            Finger finger = GetFinger(name);
            if (finger != null) finger.Target.position = position;
        }
        
        public void MoveToGrabPoint(GrabPoint grabPoint, UnityAction onComplete = null)
        {
            StartCoroutine(MoveFingersToGrabPoint(grabPoint, onComplete));
        }
        public void ResetFingers()
        {
            foreach (Finger finger in _fingers)
            {
                StartCoroutine(AnimateFinger(finger.StartPosition, finger, true));
            }
        }

        private IEnumerator MoveFingersToGrabPoint(GrabPoint target, UnityAction onComplete)
        {
            foreach (Finger finger in _fingers)
            {
                StartCoroutine(AnimateFinger(target.GetFingerPosition(finger.Name), finger));
            }
            yield return new WaitForSeconds(1.2f / _fingerSpeed);
            onComplete?.Invoke();
        }
        
        private IEnumerator AnimateFinger(Vector3 target, Finger finger, bool isLocal = false)
        {
            Vector3 startPos = isLocal ? finger.Target.localPosition : finger.Target.position;
            Vector3 endPos = target;
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * _fingerSpeed;
                if (isLocal) finger.Target.localPosition = Vector3.Lerp(startPos, endPos, t);
                else finger.Target.position = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }
        }

        public void SetFingersToGrabPoint(GrabPoint grabPoint)
        {
            foreach (Finger finger in _fingers)
            {
                finger.Target.position = grabPoint.GetFingerPosition(finger.Name);
            }
        }
    }
}

