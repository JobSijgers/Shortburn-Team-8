using UnityEngine;

namespace ArmSystem
{
    public interface IGrabable
    {
        public void Grabbed(Transform newParent);
        public void Drop();
        public string GetGrabableKey();
    }
}