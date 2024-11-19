using UnityEngine;
public interface ILatchable
{
    public Vector3 latchPos { get; }
    public Transform getTransform { get; }

    public void Latched();
}
