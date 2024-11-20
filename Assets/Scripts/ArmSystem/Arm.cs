using System;
using System.Collections;
using System.Collections.Generic;
using ArmSystem;
using UnityEngine;

public class Arm : MonoBehaviour
{
    [SerializeField] private float _lerpSpeed;
    private Coroutine _grabCoroutine;
    
    public void Grab(IGrabbable grabbable)
    {
        _grabCoroutine = StartCoroutine(GrabRoutine(grabbable));
    }

    public void Release(Transform armTransform)
    {
        if (_grabCoroutine != null)
        {
            StopCoroutine(_grabCoroutine);
            _grabCoroutine = null;
        }
        StartCoroutine(ReleaseRoutine(armTransform));
    }
    private IEnumerator GrabRoutine(IGrabbable grabbable)
    {
        Vector3 position = grabbable.GetGrabPoint().position;
        // lerp to position
        while (Vector3.Distance(transform.position, position) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * _lerpSpeed);
            yield return null;
        }

        transform.parent = grabbable.GetGrabPoint();
        grabbable.Latched();
        _grabCoroutine = null;
    }

    public IEnumerator ReleaseRoutine(Transform armTransform)
    {
        transform.parent = null;
        // lerp back to arm
        while (Vector3.Distance(transform.position, armTransform.position) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, armTransform.position, Time.deltaTime * _lerpSpeed);
            yield return null;
        }

        Destroy(gameObject);
    }
}
