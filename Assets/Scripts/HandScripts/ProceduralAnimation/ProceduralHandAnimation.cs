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
        [SerializeField] private float _fingerSpeed = 0.1f;
        [SerializeField] [Range(0, 1)] private float _fingerAnimStartPrc;

        private GrabPoint _grabPoint;

        private void Start()
        {
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
            bool[] fingerStatus = new bool[_fingers.Length];
            foreach (Finger finger in _fingers)
            {
                StartCoroutine(AnimateFinger(target.GetFingerPosition(finger.Name), finger, false,
                    () => { fingerStatus[System.Array.IndexOf(_fingers, finger)] = true; }));
            }

            while (Array.Exists(fingerStatus, status => status == false))
            {
                yield return null;
            }

            onComplete?.Invoke();
        }

        private IEnumerator AnimateFinger(Vector3 target, Finger finger,
            bool isLocal = false, UnityAction onComplete = null)
        {
            Vector3 endPos = target;
            
            while ((isLocal ? Vector3.Distance(target, finger.Target.localPosition) : Vector3.Distance(target, finger.Target.position)) > 0.001f)
            {
                if (isLocal)
                {
                    finger.Target.localPosition = Vector3.MoveTowards(finger.Target.localPosition, endPos, _fingerSpeed);
                }
                else
                {
                    finger.Target.position = Vector3.MoveTowards(finger.Target.position, endPos, _fingerSpeed);
                }
                yield return null;
            }

            if (isLocal)
            {
                finger.Target.localPosition = endPos;
            }
            else
            {
                finger.Target.position = endPos;
            }

            onComplete?.Invoke();
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