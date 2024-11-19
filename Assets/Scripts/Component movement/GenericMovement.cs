using System.Collections;
using UnityEngine;

namespace Component_movement
{
    public class GenericMovement : MonoBehaviour
    {
        [SerializeField] private float _movementSpeed;
        [SerializeField] private Vector3 _endPos;
        private Vector3 _startPos;

        public void StartMovement()
        {
            StartCoroutine(Move());
        }

        private IEnumerator Move()
        {
            Debug.Log("StartMovement");
            // lerp to end pos
            _startPos = transform.position;
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * _movementSpeed;
                transform.position = Vector3.Lerp(_startPos, _endPos, t);
                yield return null;
            }
        }
    }
}
