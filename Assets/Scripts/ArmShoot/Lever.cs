using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Lever : MonoBehaviour, ILatchable
{
    [SerializeField] private Vector3 _latchPoint;
    [SerializeField] private UnityEvent _onLatched;
    
    public Vector3 latchPos => transform.TransformPoint(_latchPoint);
    public Transform getTransform => transform;
    public void Latched()
    {
        _onLatched.Invoke();
    }
}
