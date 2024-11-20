using System;
using System.Collections;
using System.Collections.Generic;
using ArmSystem;
using UnityEngine;

public class Arm : MonoBehaviour
{
    public static Arm Instance { get; private set; }
    [SerializeField] private float _lerpSpeed;
    private void Awake()
    {
        Instance = this;
    }

    public IEnumerator Latch(IGrabbable grabbable)
    {
        Vector3 position = grabbable.GetGrabPoint().position;
        // lerp to position
        while (Vector3.Distance(transform.position, position) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * _lerpSpeed);
            yield return null;
        }

        transform.parent = grabbable.GetGrabPoint();
    }

    public IEnumerator Release(Transform armTransform)
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
