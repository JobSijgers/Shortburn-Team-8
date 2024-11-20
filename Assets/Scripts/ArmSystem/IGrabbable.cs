using UnityEngine;

namespace ArmSystem
{
    public interface IGrabbable
    {
        public Transform GetGrabPoint();

        public void Latched();
    }
}
