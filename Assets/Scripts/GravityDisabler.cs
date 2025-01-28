using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityDisabler : MonoBehaviour
{
    [SerializeField] private float _disableTime = 1f;
    private void OnEnable()
    {
        Invoke(nameof(DisableGravity), _disableTime);
    }

    private void DisableGravity()
    {
        GetComponent<Rigidbody>().isKinematic = true;

    }
}
