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
        [SerializeField] private GrabPoint _storagePoint;
        [SerializeField] private ProceduralHandAnimation _proceduralAnim;

        public GrabPoint GetStoragePoint() => _storagePoint;
        
        public void MoveToPoint(Transform grabPoint, Transform parentAfterMove,  UnityAction onComplete, GrabPoint grab = null)
        {
            StartCoroutine(MoveRoutine(grabPoint, parentAfterMove, onComplete, grab));
        }

        private IEnumerator MoveRoutine(Transform destination, Transform parentAfterMove, UnityAction onComplete, GrabPoint grab = null)
        {
            transform.SetParent(null);
            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;

            float t = 0f;
            while (t <= 1)
            {
                t += Time.deltaTime / _moveSpeed;
                float a = _moveCurve.Evaluate(t);
                transform.position = Vector3.Lerp(startPos, grab != null ? grab.PathCreator.path.GetPoint(0) : destination.position, a);
                transform.rotation = Quaternion.Slerp(startRot, grab != null ? grab.PathCreator.path.GetRotation(0) : destination.rotation, a);
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

        public void ResetFingers()
        {
            _proceduralAnim.ResetFingers();
        }
    }
}