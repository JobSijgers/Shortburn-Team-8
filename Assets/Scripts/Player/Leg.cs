using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class Leg : MonoBehaviour
    {
        public void ReturnLeg(Vector3 returnPosition)
        {
            StartCoroutine(Latch(returnPosition));
        }

        private IEnumerator Latch(Vector3 position)
        {
            // lerp to position
            GetComponent<Rigidbody>().isKinematic = true;
            while (Vector3.Distance(transform.position, position) > 0.1f)
            {
                transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * 10);
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}