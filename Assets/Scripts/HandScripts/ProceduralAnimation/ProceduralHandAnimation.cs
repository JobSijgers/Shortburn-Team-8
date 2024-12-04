using UnityEngine;
using System;
using System.Collections;
using HandScripts.Grab;
using PathCreation.Examples;

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
        [SerializeField] private float _fingerSpeed = 1;
        [SerializeField] [Range(0, 1)]private float _fingerAnimStartPrc;
        [SerializeField] private GrabPoint _grabPoint;

        private PathFollower _pathFollower;
        private void Start()
        {
            StartAnimation(_grabPoint);
            _pathFollower = GetComponent<PathFollower>();
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
            Vector3 endPosition = point.PathCreator.path.GetPoint(0);
            Quaternion startRotation = transform.rotation;
            Quaternion endRotation = point.PathCreator.path.GetRotation(0);
            bool startFingers = true;
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * _animationSpeed;
                transform.position = Vector3.Slerp(startPosition, endPosition, t);
                transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
                yield return null;
            }
            _pathFollower.pathCreator = point.PathCreator;
            
        }
        private IEnumerator AnimateFinger(GrabPoint grabPoint, Finger finger)
        {
            Vector3 StartPos = finger.Target.position;
            Vector3 EndPos = grabPoint.GetFingerPosition(finger.Name);
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * _fingerSpeed;
                finger.Target.position = Vector3.Slerp(StartPos, EndPos, t);
                yield return null;
            }
        }
    }
}

