using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace HandScripts.Core
{
    public class Hand : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 1f;
        [SerializeField] private AnimationCurve _moveCurve;
        [SerializeField] private Transform _storagePoint;

        public Transform GetStoragePoint() => _storagePoint;
        
        public void MoveToPoint(Transform grabPoint, Transform parentAfterMove, UnityAction onComplete)
        {
            StartCoroutine(MoveRoutine(grabPoint, parentAfterMove, onComplete));
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
    }
}