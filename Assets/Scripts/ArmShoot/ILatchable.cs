using UnityEngine;
public interface ILatchable
{
    public Vector3 latchPos { get; }

    public void Latched();
}
