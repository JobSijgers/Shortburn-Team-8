using Unity.Mathematics;
using UnityEngine;

public class Lever : MonoBehaviour, ILatchable
{
    [SerializeField] private Vector3 _latchPoint;
    
    public Vector3 latchPos => transform.TransformPoint(_latchPoint);
}
