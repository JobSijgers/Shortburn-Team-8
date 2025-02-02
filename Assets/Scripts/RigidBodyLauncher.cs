using System.Collections;
using HandScripts.Grab;
using UnityEngine;

public class RigidBodyLauncher : MonoBehaviour
{
    [SerializeField] private float _launchForce;
    [SerializeField] private float _disableRigidbodyTime;

    public void Launch(IHandGrabable grabable)
    {
        Rigidbody rb = grabable.GetRigidbody();
        rb.isKinematic = false;
        rb.velocity = Vector3.zero;
        rb.AddForce(transform.forward * _launchForce, ForceMode.Impulse);
        StartCoroutine(DisableRigidbody(grabable));
    }

    private IEnumerator DisableRigidbody(IHandGrabable grabable)
    {
        yield return new WaitForSeconds(_disableRigidbodyTime);
        grabable.GetRigidbody().isKinematic = true;
    }
}