using HandScripts.Grab;
using UnityEngine;

namespace HandScripts.Core
{
    public interface IHandInteractable
    {
        GrabPoint GetGrabPoint();
        EInteractType GetInteractType();
        Transform GetObjectTransform();
    }
}