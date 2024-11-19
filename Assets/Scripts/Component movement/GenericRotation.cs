using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericRotation : MonoBehaviour
{
    [SerializeField] private float _movementSpeed;
    [SerializeField] private Quaternion _endRot;
    private Quaternion _startRot;

    public void StartMovement()
    {
        StartCoroutine(Move());
    }
    public IEnumerator Move()
    {
        // lerp to end pos
        _startRot = transform.rotation;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * _movementSpeed;
            transform.rotation = Quaternion.Lerp(_startRot, _endRot, t);
            yield return null;
        }
    }
}