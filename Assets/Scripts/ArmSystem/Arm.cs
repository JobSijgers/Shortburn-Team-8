using System.Collections;
using ArmSystem;
using UnityEngine;

public class Arm : MonoBehaviour
{
    [SerializeField] private float _lerpSpeed;
    private Coroutine _grabCoroutine;
    public IGrabable _grabbedObject;
    

    public void Interact(IInteractable interactable)
    {
        _grabCoroutine = StartCoroutine(InteractRoutine(interactable));
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

    public void GrabObject(Transform grabPoint, Transform armHoldPosition, IGrabable grabable)
    {
        StartCoroutine(GrabObjectRoutine(grabPoint, armHoldPosition, grabable));
    }

    public void DepositObject(Transform armHoldPosition, IDeposit depo)
    {
        StartCoroutine(DepositObjectRoutine(armHoldPosition, depo));
    }

    private IEnumerator DepositObjectRoutine(Transform armHoldPosition, IDeposit depo)
    {
        yield return StartCoroutine(MoveArmToTransform(depo.GetDepositPoint()));
        depo.Deposited();
        yield return StartCoroutine(MoveArmToTransform(armHoldPosition));
        Destroy(gameObject);
    }

    private IEnumerator InteractRoutine(IInteractable interactable)
    {
        yield return StartCoroutine(MoveArmToTransform(interactable.GetGrabPoint()));
        transform.parent = interactable.GetGrabPoint();
        interactable.Interacted();
        _grabCoroutine = null;
    }

    private IEnumerator ReleaseRoutine(Transform armTransform)
    {
        transform.parent = null;
        // lerp back to arm
        yield return StartCoroutine(MoveArmToTransform(armTransform));
        Destroy(gameObject);
    }

    private IEnumerator GrabObjectRoutine(Transform grabPoint, Transform armHoldPosition, IGrabable grabable)
    {
        yield return StartCoroutine(MoveArmToTransform(grabPoint));
        grabable.Grabbed(transform);
        _grabbedObject = grabable;
        yield return StartCoroutine(MoveArmToTransform(armHoldPosition));
        transform.SetParent(armHoldPosition);
    }

    private IEnumerator MoveArmToTransform(Transform destination)
    {
        float time = 0;
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        while (time < 1)
        {
            time += Time.deltaTime * _lerpSpeed;
            transform.position = Vector3.Lerp(startPosition, destination.position, time);
            transform.rotation = Quaternion.Lerp(startRotation, destination.rotation, time);
            yield return null;
        }
    }
    
    public void DropObject()
    {
        if (_grabbedObject == null)
            return;
        
        _grabbedObject.Drop();
        _grabbedObject = null;
        Destroy(gameObject);
    }
}