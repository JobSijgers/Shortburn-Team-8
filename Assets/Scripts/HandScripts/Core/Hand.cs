using System.Collections;
using HandScripts.Grab;
using HandScripts.ProceduralAnimation;
using PathCreation;
using UnityEngine;
using UnityEngine.Events;

namespace HandScripts.Core
{
    public class Hand : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 1f;
        [SerializeField] private AnimationCurve _moveCurve;
        [SerializeField] private GrabPoint _storagePoint;
        [SerializeField] private ProceduralHandAnimation _proceduralAnim;

        public GrabPoint GetStoragePoint() => _storagePoint;

        public void MoveToPoint(Transform destination, Transform parentAfterMove, UnityAction onComplete)
        {
            StartCoroutine(MoveRoutine(destination, parentAfterMove, onComplete));
        }

        public void MoveToGrabPoint(GrabPoint target, Transform parentAfterMove, UnityAction onComplete)
        {
            StartCoroutine(MoveToGrabPointRoutine(target, parentAfterMove, onComplete));
        }

        private IEnumerator MoveToGrabPointRoutine(GrabPoint target, Transform parentAfterMove, UnityAction onComplete)
        {
            Vector3 splineStart = target.PathCreator.path.GetPointAtDistance(0);
            Quaternion splineRot = target.UsePathRotation
                ? target.PathCreator.path.GetRotationAtDistance(0)
                : target.GrabPointTransform.rotation;

            yield return StartCoroutine(MoveRoutine(splineStart, splineRot, null, null));
            yield return StartCoroutine(FollowPathRoutine(target.PathCreator, target.UsePathRotation, null));
            yield return StartCoroutine(MoveRoutine(target.GrabPointTransform, null, null));
            _proceduralAnim.MoveToGrabPoint(target, () => onComplete?.Invoke());
            transform.SetParent(parentAfterMove);
        }

        private IEnumerator MoveRoutine(Vector3 endPosition, Quaternion endRotation, Transform parentAfterMove,
            UnityAction onComplete)
        {
            transform.SetParent(null);
            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;

            float t = 0f;
            while (t <= 1)
            {
                t += Time.deltaTime / _moveSpeed;
                float a = _moveCurve.Evaluate(t);
                transform.position = Vector3.Lerp(startPos, endPosition, a);
                transform.rotation = Quaternion.Slerp(startRot, endRotation, a);
                yield return null;
            }

            transform.SetParent(parentAfterMove);
            onComplete?.Invoke();
        }

        private IEnumerator MoveRoutine(Transform destination, Transform parentAfterMove, UnityAction onComplete)
        {
            transform.SetParent(null);
            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;
            float t = 0f;
            while (t <= 1)
            {
                t += Time.deltaTime / _moveSpeed;
                float a = _moveCurve.Evaluate(t);
                transform.position = Vector3.Lerp(startPos, destination.position, a);
                transform.rotation = Quaternion.Slerp(startRot, destination.rotation, a);
                yield return null;
            }

            transform.SetParent(parentAfterMove);
            onComplete?.Invoke();
        }

        private IEnumerator FollowPathRoutine(PathCreator path, bool usePathRotation, UnityAction onComplete)
        {
            float distanceTravelled = 0;

            while (distanceTravelled < path.path.length)
            {
                distanceTravelled += _moveSpeed * Time.deltaTime * 5;
                transform.position = path.path.GetPointAtDistance(distanceTravelled, EndOfPathInstruction.Stop);
                if (usePathRotation)
                {
                    transform.rotation = path.path.GetRotationAtDistance(distanceTravelled, EndOfPathInstruction.Stop);
                }

                yield return null;
            }

            onComplete?.Invoke();
        }

        public void Grab(GrabPoint grabPoint)
        {
            _proceduralAnim.SetFingersToGrabPoint(grabPoint);
        }
        
        public void ResetFingers()
        {
            _proceduralAnim.ResetFingers();
        }
    }
}