using System.Collections;
using ArmSystem;
using UnityEngine;

public class Arm : MonoBehaviour
{
    [SerializeField] private float _lerpSpeed;
    private Coroutine _grabCoroutine;
    private IGrabable _grabbedObject;

    public void Interact(IInteractable interactable)
    {
        _grabCoroutine = StartCoroutine(GrabRoutine(interactable));
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

    private IEnumerator GrabRoutine(IInteractable interactable)
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
    }

    private IEnumerator MoveArmToTransform(Transform destination)
    {
        while (Vector3.Distance(transform.position, destination.position) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, destination.position, Time.deltaTime * _lerpSpeed);
            yield return null;
        }
    }
}