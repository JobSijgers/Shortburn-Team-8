using System.Collections;
using HandScripts.Grab;
using HandScripts.ProceduralAnimation;
using UnityEngine;
using UnityEngine.Events;

namespace HandScripts.Core
{
    public class Hand : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 1f;
        [SerializeField] private AnimationCurve _moveCurve;
        [SerializeField] private Transform _storagePoint;
        [SerializeField] private ProceduralHandAnimation _proceduralAnim;

        public Transform GetStoragePoint() => _storagePoint;
        
        public void MoveToPoint(Transform grabPoint, Transform parentAfterMove, UnityAction onComplete)
        {
            GrabPoint grab = grabPoint.GetComponentInChildren<GrabPoint>();
            StartCoroutine(MoveRoutine(grabPoint, parentAfterMove, onComplete, grab));
        }

        private IEnumerator MoveRoutine(Transform destination, Transform parentAfterMove, UnityAction onComplete, GrabPoint grab = null)
        {
            transform.SetParent(null);
            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;
            Vector3 endPos = destination.position;
            Quaternion endRot = destination.rotation;
            if (grab != null)
            {
                endPos = grab.PathCreator.path.GetPoint(0);
                endRot = grab.PathCreator.path.GetRotation(0);
            }

            float t = 0f;
            while (t <= 1)
            {
                t += Time.deltaTime / _moveSpeed;
                float a = _moveCurve.Evaluate(t);
                transform.position = Vector3.Lerp(startPos, endPos, a);
                transform.rotation = Quaternion.Slerp(startRot, endRot, a);
                yield return null;
            }
            transform.SetParent(parentAfterMove);
            if (grab != null)
            {
                _proceduralAnim.SetGrabPoint(grab);
                _proceduralAnim.GetPathFollower().FollowPath(grab.PathCreator, () =>
                {
                    _proceduralAnim.MoveToGrabPoint(onComplete);
                });
            }
            else
            {
                onComplete?.Invoke();
            }
        }
    }
}