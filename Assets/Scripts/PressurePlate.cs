using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    public UnityEvent OnPressurePlate;
    public UnityEvent OnUnPressurePlate;
    private int _amount;
    private Rigidbody _rigidbody;
    private SpringJoint _springJoint;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _springJoint = GetComponent<SpringJoint>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        _amount++;
        if (_amount != 1) 
            return;
        _springJoint.minDistance = 0.2f;
        OnPressurePlate.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) 
            return;
        _amount--;
        if (_amount != 0) 
            return;
        _springJoint.minDistance = 0;
        _rigidbody.WakeUp();
        OnUnPressurePlate.Invoke();
    }
}