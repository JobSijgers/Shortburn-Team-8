using UnityEngine;

namespace HandScripts.Core
{
    public interface IHandInteractable
    {
        Transform GetHeldPoint();
        EInteractType GetInteractType();
        Transform GetObjectTransform();
    }
}