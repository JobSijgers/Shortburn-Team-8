using UnityEngine;

namespace ArmSystem
{
    public interface IInteractable
    {
        public Transform GetGrabPoint();

        public void Interacted();
    }
}
