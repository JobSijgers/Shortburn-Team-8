using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private float _pressSpeed;
    [SerializeField] private Vector3 _moveAmount;
    [SerializeField] private Material _pressurePlateMaterial;
    public UnityEvent OnPressurePlate;
    public UnityEvent OnUnPressurePlate;
    private int _amount;

    private Vector3 _startPos;
    private Coroutine _moveRoutine;
    private MeshRenderer _meshRenderer;

    private void Start()
    {
        _startPos = transform.localPosition;
        _meshRenderer = GetComponent<MeshRenderer>();
        MovePlate(Vector3.up);
    }

    public void MovePlate(Vector3 moveDir)
    {
        Vector3 endPos = _startPos + Vector3.Scale(moveDir, _moveAmount) / 2;
        if (_moveRoutine != null) StopCoroutine(_moveRoutine);
        _moveRoutine = StartCoroutine(MoveRoutine(transform.localPosition, endPos));
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        _amount++;
        if (_amount != 1) 
            return;
        MovePlate(-Vector3.up);
        OnPressurePlate.Invoke();
        
        // copy material and turn on emission (little scuffed)
        Material newMaterial = new Material(_pressurePlateMaterial);
        newMaterial.EnableKeyword("_EMISSION");
        _meshRenderer.material = newMaterial;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) 
            return;
        _amount--;
        if (_amount != 0) 
            return;
        MovePlate(Vector3.up);
        _meshRenderer.material = _pressurePlateMaterial;
        OnUnPressurePlate.Invoke();
    }

    private IEnumerator MoveRoutine(Vector3 startPos, Vector3 endPos)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * _pressSpeed;
            transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
        _moveRoutine = null;
    }
}